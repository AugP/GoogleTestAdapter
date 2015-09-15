﻿using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using GoogleTestAdapter;

namespace GoogleTestAdapterVSIX
{

    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    [ProvideOptionPage(typeof(GeneralOptionsDialogPage), GoogleTestAdapterOptions.CATEGORY_NAME, GoogleTestAdapterOptions.PAGE_GENERAL_NAME, 0, 0, true)]
    [ProvideOptionPage(typeof(ParallelizationOptionsDialogPage), GoogleTestAdapterOptions.CATEGORY_NAME, GoogleTestAdapterOptions.PAGE_PARALLELIZATION_NAME, 0, 0, true)]
    [ProvideAutoLoad(UIContextGuids80.SolutionExists)]
    public sealed class GoogleTestExtensionOptionsPage : Package
    {
        public const string PackageGuidString = "e7c90fcb-0943-4908-9ae8-3b6a9d22ec9e";

        override protected void Initialize()
        {
            base.Initialize();

            DialogPage Page = GetDialogPage(typeof(GeneralOptionsDialogPage));
            Page.SaveSettingsToStorage();
            Page = GetDialogPage(typeof(ParallelizationOptionsDialogPage));
            Page.SaveSettingsToStorage();
        }

    }

    public class GeneralOptionsDialogPage : DialogPage
    {

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_PRINT_TEST_OUTPUT)]
        [Description("Print the output generated by the test executable(s) to the Test Output window.")]
        public bool PrintTestOutput { get; set; } = GoogleTestAdapterOptions.OPTION_PRINT_TEST_OUTPUT_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_TEST_DISCOVERY_REGEX)]
        [Description(@"If non-empty, this regex will be used to discover the executables containing your tests. Default regex: " + GoogleTestAdapter.Constants.TEST_FINDER_REGEX)]
        public string TestDiscoveryRegex { get; set; } = GoogleTestAdapterOptions.OPTION_TEST_DISCOVERY_REGEX_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_RUN_DISABLED_TESTS)]
        [Description("If true, all (selected) tests will be run, even if they have been disabled.")]
        public bool RunDisabledTests { get; set; } = GoogleTestAdapterOptions.OPTION_RUN_DISABLED_TESTS_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_NR_OF_TEST_REPETITIONS)]
        [Description("Tests will be run for the selected number of times (-1: infinite).")]
        public int NrOfTestRepetitions { get; set; } = GoogleTestAdapterOptions.OPTION_NR_OF_TEST_REPETITIONS_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_SHUFFLE_TESTS)]
        [Description("If true, tests will be executed in random order.")]
        public bool ShuffleTests { get; set; } = GoogleTestAdapterOptions.OPTION_SHUFFLE_TESTS_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_USER_DEBUG_MODE)]
        [Description("If true, debug output will be printed to the test console.")]
        public bool UserDebugMode { get; set; } = GoogleTestAdapterOptions.OPTION_USER_DEBUG_MODE_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_TRAITS_REGEXES_BEFORE)]
        [Description("Allows to add traits to testcases matching a regex. "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + " separates the regex from the traits, the trait's name and value are separated by "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + ", and each pair of regex and trait is separated by "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_PAIR_SEPARATOR + ".\nExample: " + @"MySuite\.*"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + "Type"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + "Small"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_PAIR_SEPARATOR + @"MySuite2\.*|MySuite3\.*"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + "Type"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + "Medium")]
        public string TraitsRegexesBefore { get; set; } = GoogleTestAdapterOptions.OPTION_TRAITS_REGEXES_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_TRAITS_REGEXES_AFTER)]
        [Description("Allows to override/add traits for testcases matching a regex. "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + " separates the regex from the traits, the trait's name and value are separated by "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + ", and each pair of regex and trait is separated by "
            + GoogleTestAdapterOptions.TRAITS_REGEXES_PAIR_SEPARATOR + ".\nExample: " + @"MySuite\.*"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + "Type"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + "Small"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_PAIR_SEPARATOR + @"MySuite2\.*|MySuite3\.*"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_REGEX_SEPARATOR + "Type"
            + GoogleTestAdapterOptions.TRAITS_REGEXES_TRAIT_SEPARATOR + "Medium")]
        public string TraitsRegexesAfter { get; set; } = GoogleTestAdapterOptions.OPTION_TRAITS_REGEXES_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_ADDITIONAL_TEST_EXECUTION_PARAM)]
        [Description("Additional parameters for Google Test executable.\n" + GoogleTestAdapterOptions.DESCRIPTION_OF_PLACEHOLDERS)]
        public string AdditionalTestExecutionParams { get; set; } = GoogleTestAdapterOptions.OPTION_ADDITIONAL_TEST_EXECUTION_PARAM_DEFAULT_VALUE;

    }

    public class ParallelizationOptionsDialogPage : DialogPage
    {

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_ENABLE_PARALLEL_TEST_EXECUTION)]
        [Description("Enable parallel test execution (experimental!)")]
        public bool EnableParallelTestExecution { get; set; } = GoogleTestAdapterOptions.OPTION_PRINT_TEST_OUTPUT_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_MAX_NR_OF_THREADS)]
        [Description("Maximum number of threads used for test execution. 0 = all available threads.")]
        public int MaxNrOfThreads { get; set; } = GoogleTestAdapterOptions.OPTION_MAX_NR_OF_THREADS_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_TEST_SETUP_BATCH)]
        [Description("Batch file to be executed before each parallel test execution.\n" + GoogleTestAdapterOptions.DESCRIPTION_OF_PLACEHOLDERS)]
        public string BatchForTestSetup { get; set; } = GoogleTestAdapterOptions.OPTION_TEST_SETUP_BATCH_DEFAULT_VALUE;

        [Category(GoogleTestAdapterOptions.CATEGORY_NAME)]
        [DisplayName(GoogleTestAdapterOptions.OPTION_TEST_TEARDOWN_BATCH)]
        [Description("Batch file to be executed after each parallel test execution.\n" + GoogleTestAdapterOptions.DESCRIPTION_OF_PLACEHOLDERS)]
        public string BatchForTestTeardown { get; set; } = GoogleTestAdapterOptions.OPTION_TEST_TEARDOWN_BATCH_DEFAULT_VALUE;

    }

}