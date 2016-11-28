/**
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{
    [TestClass]
    public class CommandLineTest
    {
        [TestMethod]
        public void testGetOptionProperties()
        {
            string[] args = new string[] { "-Dparam1=value1", "-Dparam2=value2", "-Dparam3", "-Dparam4=value4", "-D", "--property", "foo=bar" };

            Options options = new Options();
            options.addOption(OptionBuilder.withValueSeparator().hasOptionalArgs(2).create('D'));
            options.addOption(OptionBuilder.withValueSeparator().hasArgs(2).withLongOpt("property").create());

            Parser parser = new GnuParser();
            CommandLine cl = parser.parse(options, args);

            Dictionary<string, string> props = cl.getOptionProperties("D");
            Assert.IsNotNull(props, "null properties");
            Assert.AreEqual(4, props.Count, "number of properties in " + props);
            Assert.AreEqual("value1", props["param1"], "property 1");
            Assert.AreEqual("value2", props["param2"], "property 2");
            Assert.AreEqual("true", props["param3"], "property 3");
            Assert.AreEqual("value4", props["param4"], "property 4");

            Assert.AreEqual("bar", cl.getOptionProperties("property")["foo"], "property with long format");
        }

        [TestMethod]
        public void testGetOptions()
        {
            CommandLine cmd = new CommandLine();
            Assert.IsNotNull(cmd.getOptions());
            Assert.AreEqual(0, cmd.getOptions().Length);

            cmd.addOption(new Option("a", null));
            cmd.addOption(new Option("b", null));
            cmd.addOption(new Option("c", null));

            Assert.AreEqual(3, cmd.getOptions().Length);
        }

        [TestMethod]
        public void testGetParsedOptionValue()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.hasArg().withType(typeof(ValueType)).create("i"));
            options.addOption(OptionBuilder.hasArg().create("f"));

            CommandLineParser parser = new DefaultParser();
            CommandLine cmd = parser.parse(options, new string[] { "-i", "123", "-f", "foo" });

            Assert.AreEqual(123, int.Parse(cmd.getParsedOptionValue("i").ToString()));
            Assert.AreEqual("foo", cmd.getParsedOptionValue("f"));
        }
    }
}
