// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.Linq;
using System.Management.Automation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PSRule.Configuration;
using PSRule.Definitions;
using PSRule.Definitions.Rules;
using PSRule.Pipeline;
using PSRule.Pipeline.Output;
using PSRule.Rules;
using Xunit;

namespace PSRule
{
    public sealed class OutputWriterTests
    {
        [Fact]
        public void Sarif()
        {
            var option = GetOption();
            option.Output.SarifProblemsOnly = false;
            option.Repository.Url = "https://github.com/microsoft/PSRule.UnitTest";
            var output = new TestWriter(option);
            var result = new InvokeResult();
            result.Add(GetPass());
            result.Add(GetFail());
            result.Add(GetFail("rid-003", SeverityLevel.Warning));
            result.Add(GetFail("rid-004", SeverityLevel.Information));
            var writer = new SarifOutputWriter(null, output, option);
            writer.Begin();
            writer.WriteObject(result, false);
            writer.End();

            var actual = JsonConvert.DeserializeObject<JObject>(output.Output.OfType<string>().FirstOrDefault());
            Assert.NotNull(actual);
            Assert.Equal("PSRule", actual["runs"][0]["tool"]["driver"]["name"]);
            Assert.Equal("0.0.1", actual["runs"][0]["tool"]["driver"]["semanticVersion"].Value<string>().Split('+')[0]);
            Assert.Equal("https://github.com/microsoft/PSRule.UnitTest", actual["runs"][0]["versionControlProvenance"][0]["repositoryUri"].Value<string>());

            // Pass
            Assert.Equal("TestModule\\rule-001", actual["runs"][0]["results"][0]["ruleId"]);
            Assert.Equal("none", actual["runs"][0]["results"][0]["level"]);

            // Fail with error
            Assert.Equal("rid-002", actual["runs"][0]["results"][1]["ruleId"]);
            Assert.Equal("error", actual["runs"][0]["results"][1]["level"]);

            // Fail with warning
            Assert.Equal("rid-003", actual["runs"][0]["results"][2]["ruleId"]);
            Assert.Null(actual["runs"][0]["results"][2]["level"]);

            // Fail with note
            Assert.Equal("rid-004", actual["runs"][0]["results"][3]["ruleId"]);
            Assert.Equal("note", actual["runs"][0]["results"][3]["level"]);
        }

        [Fact]
        public void SarifProblemsOnly()
        {
            var option = GetOption();
            var output = new TestWriter(option);
            var result = new InvokeResult();
            result.Add(GetPass());
            result.Add(GetFail());
            result.Add(GetFail("rid-003", SeverityLevel.Warning));
            result.Add(GetFail("rid-004", SeverityLevel.Information));
            var writer = new SarifOutputWriter(null, output, option);
            writer.Begin();
            writer.WriteObject(result, false);
            writer.End();

            var actual = JsonConvert.DeserializeObject<JObject>(output.Output.OfType<string>().FirstOrDefault());
            Assert.NotNull(actual);
            Assert.Equal("PSRule", actual["runs"][0]["tool"]["driver"]["name"]);
            Assert.Equal("0.0.1", actual["runs"][0]["tool"]["driver"]["semanticVersion"].Value<string>().Split('+')[0]);

            // Fail with error
            Assert.Equal("rid-002", actual["runs"][0]["results"][0]["ruleId"]);
            Assert.Equal("error", actual["runs"][0]["results"][0]["level"]);

            // Fail with warning
            Assert.Equal("rid-003", actual["runs"][0]["results"][1]["ruleId"]);
            Assert.Null(actual["runs"][0]["results"][1]["level"]);

            // Fail with note
            Assert.Equal("rid-004", actual["runs"][0]["results"][2]["ruleId"]);
            Assert.Equal("note", actual["runs"][0]["results"][2]["level"]);
        }

        #region Helper methods

        private static RuleRecord GetPass()
        {
            return new RuleRecord(
                runId: "run-001",
                ruleId: ResourceId.Parse("TestModule\\rule-001"),
                @ref: null,
                targetObject: new TargetObject(new PSObject()),
                targetName: "TestObject1",
                targetType: "TestType",
                tag: new ResourceTags(),
                info: new RuleHelpInfo("rule-001", "Rule 001", "TestModule")
                {
                    Synopsis = "This is rule 001.",
                    Recommendation = "Recommendation for rule 001",
                },
                field: new Hashtable(),
                level: SeverityLevel.Error,
                extent: null,
                outcome: RuleOutcome.Pass,
                reason: RuleOutcomeReason.Processed
            );
        }

        private static RuleRecord GetFail(string ruleRef = "rid-002", SeverityLevel level = SeverityLevel.Error)
        {
            return new RuleRecord(
                runId: "run-001",
                ruleId: ResourceId.Parse("TestModule\\rule-002"),
                @ref: ruleRef,
                targetObject: new TargetObject(new PSObject()),
                targetName: "TestObject1",
                targetType: "TestType",
                tag: new ResourceTags(),
                info: new RuleHelpInfo("rule-002", "Rule 002", "TestModule")
                {
                    Synopsis = "This is rule 002.",
                    Recommendation = "Recommendation for rule 002",
                },
                field: new Hashtable(),
                level: level,
                extent: null,
                outcome: RuleOutcome.Fail,
                reason: RuleOutcomeReason.Processed
            );
        }

        private static PSRuleOption GetOption()
        {
            return new PSRuleOption();
        }

        #endregion Helper methods
    }
}
