﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GoogleTestAdapter.Helpers;
using GoogleTestAdapter.Scheduling;
using GoogleTestAdapter.TestResults;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;

namespace GoogleTestAdapter.Execution
{
    class SequentialTestRunner : AbstractGoogleTestAdapterClass, IGoogleTestRunner
    {
        public bool Canceled { get; set; } = false;

        internal SequentialTestRunner(AbstractOptions options) : base(options) { }

        public void RunTests(bool runAllTestCases, IEnumerable<TestCase> allTestCases, IEnumerable<TestCase> testCasesToRun, IRunContext runContext, IFrameworkHandle handle, string userParameters)
        {
            if (userParameters == null)
            {
                throw new ArgumentNullException(nameof(userParameters));
            }

            IDictionary<string, List<TestCase>> groupedTestCases = GoogleTestExecutor.GroupTestcasesByExecutable(testCasesToRun);
            TestCase[] allTestCasesAsArray = allTestCases as TestCase[] ?? allTestCases.ToArray();
            foreach (string executable in groupedTestCases.Keys)
            {
                if (Canceled)
                {
                    break;
                }
                RunTestsFromExecutable(runAllTestCases, executable, allTestCasesAsArray, groupedTestCases[executable], runContext, handle, userParameters);
            }

        }

        // ReSharper disable once UnusedParameter.Local
        private void RunTestsFromExecutable(bool runAllTestCases, string executable, IEnumerable<TestCase> allTestCases, IEnumerable<TestCase> testCasesToRun, IRunContext runContext, IFrameworkHandle handle, string userParameters)
        {
            TestCase[] testCasesToRunAsArray = testCasesToRun as TestCase[] ?? testCasesToRun.ToArray();
            foreach (TestCase testCase in testCasesToRunAsArray)
            {
                handle.RecordStart(testCase);
            }

            string resultXmlFile = Path.GetTempFileName();
            string workingDir = Path.GetDirectoryName(executable);
            TestResultReporter reporter = new TestResultReporter(handle);
            foreach (CommandLineGenerator.Args arguments in new CommandLineGenerator(runAllTestCases, executable.Length, allTestCases, testCasesToRunAsArray, resultXmlFile, handle, Options, userParameters).GetCommandLines())
            {
                List<string> consoleOutput = ProcessUtils.GetOutputOfCommand(handle, workingDir, executable, arguments.CommandLine, Options.PrintTestOutput && !Options.ParallelTestExecution, false, runContext, handle);
                IEnumerable<TestResult> results = CollectTestResults(resultXmlFile, consoleOutput, arguments.TestCases,
                    handle);
                reporter.ReportTestResults(results);

                TestDurationSerializer serializer = new TestDurationSerializer();
                serializer.UpdateTestDurations(results);
            }
        }

        private List<TestResult> CollectTestResults(string resultXmlFile, List<string> consoleOutput, IEnumerable<TestCase> testCasesRun, IFrameworkHandle handle)
        {
            List<TestResult> testResults = new List<TestResult>();

            var testCasesRunAsArray = testCasesRun as TestCase[] ?? testCasesRun.ToArray();
            XmlTestResultParser xmlParser = new XmlTestResultParser(resultXmlFile, testCasesRunAsArray, handle);
            testResults.AddRange(xmlParser.GetTestResults());

            if (testResults.Count < testCasesRunAsArray.Length)
            {
                StandardOutputTestResultParser consoleParser = new StandardOutputTestResultParser(consoleOutput, testCasesRunAsArray, handle);
                List<TestResult> consoleResults = consoleParser.GetTestResults();
                foreach (TestResult testResult in consoleResults.Where(tr => !testResults.Exists(tr2 => tr.TestCase.FullyQualifiedName == tr2.TestCase.FullyQualifiedName)))
                {
                    testResults.Add(testResult);
                }

                if (testResults.Count < testCasesRunAsArray.Length)
                {
                    foreach (TestCase testCase in testCasesRunAsArray.Where(tc => !testResults.Exists(tr => tr.TestCase.FullyQualifiedName == tc.FullyQualifiedName)))
                    {
                        string errorMsg = consoleParser.CrashedTestCase == null ? ""
                            : "reason is probably a crash of test " + consoleParser.CrashedTestCase.DisplayName;
                        testResults.Add(new TestResult(testCase)
                        {
                            ComputerName = Environment.MachineName,
                            Outcome = TestOutcome.Skipped,
                            ErrorMessage = errorMsg
                        });
                    }
                }
            }

            return testResults;
        }

    }
}
