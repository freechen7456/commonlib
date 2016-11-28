/*
 * Licensed to the Apache Software Foundation (ASF) under one or more
 * contributor license agreements.  See the NOTICE file distributed with
 * this work for additional information regarding copyright ownership.
 * The ASF licenses this file to You under the Apache License, Version 2.0
 * (the "License"); you may not use this file except in compliance with
 * the License.  You may obtain a copy of the License at
 * 
 *      http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.commons.cli;

namespace org.apache.commons.cli.bug
{
    [TestClass]
    public class BugCLI71Test
    {
        private Options options;
        private CommandLineParser parser;

        [TestInitialize]
        public void setUp()
        {
            options = new Options();

            Option algorithm = new Option("a", "algo", true, "the algorithm which it to perform executing");
            algorithm.setArgName("algorithm name");
            options.addOption(algorithm);

            Option key = new Option("k", "key", true, "the key the setted algorithm uses to process");
            algorithm.setArgName("value");
            options.addOption(key);

            parser = new PosixParser();
        }

        [TestMethod]
        public void testBasic()
        {
            string[] args = new string[] { "-a", "Caesar", "-k", "A" };
            CommandLine line = parser.parse(options, args);
            Assert.AreEqual("Caesar", line.getOptionValue("a"));
            Assert.AreEqual("A", line.getOptionValue("k"));
        }

        [TestMethod]
        public void testMistakenArgument()
        {
            string[] args = new string[] { "-a", "Caesar", "-k", "A" };
            CommandLine line = parser.parse(options, args);
            args = new String[] { "-a", "Caesar", "-k", "a" };
            line = parser.parse(options, args);
            Assert.AreEqual("Caesar", line.getOptionValue("a"));
            Assert.AreEqual("a", line.getOptionValue("k"));
        }

        [TestMethod]
        public void testLackOfError()
        {
            string[] args = new string[] { "-k", "-a", "Caesar" };
            try
            {
                parser.parse(options, args);
                Assert.Fail("MissingArgumentException expected");
            }
            catch (MissingArgumentException e)
            {
                Assert.AreEqual("k", e.getOption().getOpt(), "option missing an argument");
            }
        }

        [TestMethod]
        public void testGetsDefaultIfOptional()
        {
            string[] args = new string[] { "-k", "-a", "Caesar" };
            options.getOption("k").setOptionalArg(true);
            CommandLine line = parser.parse(options, args);

            Assert.AreEqual("Caesar", line.getOptionValue("a"));
            Assert.AreEqual("a", line.getOptionValue('k', "a"));
        }

    }
}
