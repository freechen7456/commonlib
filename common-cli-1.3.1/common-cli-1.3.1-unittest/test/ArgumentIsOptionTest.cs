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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{
    [TestClass]
    public class ArgumentIsOptionTest
    {
        private Options options = null;
        private CommandLineParser parser = null;

        [TestInitialize]
        public void setUp()
        {
            options = new Options().addOption("p", false, "Option p").addOption("attr", true, "Option accepts argument");

            parser = new PosixParser();
        }

        [TestMethod]
        public void testOptionAndOptionWithArgument()
        {
            string[] args = new string[]{
                "-p",
                "-attr",
                "p"
        };

            CommandLine cl = parser.parse(options, args);
            Assert.IsTrue(cl.hasOption("p"), "Confirm -p is set");
            Assert.IsTrue(cl.hasOption("attr"), "Confirm -attr is set");
            Assert.IsTrue(cl.getOptionValue("attr").Equals("p"), "Confirm arg of -attr");
            Assert.IsTrue(cl.getArgs().Length == 0, "Confirm all arguments recognized");
        }

        [TestMethod]
        public void testOptionWithArgument()
        {
            string[] args = new string[]{
                "-attr",
                "p"
        };

            CommandLine cl = parser.parse(options, args);
            Assert.IsFalse(cl.hasOption("p"), "Confirm -p is set");
            Assert.IsTrue(cl.hasOption("attr"), "Confirm -attr is set");
            Assert.IsTrue(cl.getOptionValue("attr").Equals("p"), "Confirm arg of -attr");
            Assert.IsTrue(cl.getArgs().Length == 0, "Confirm all arguments recognized");
        }

        [TestMethod]
        public void testOption()
        {
            string[] args = new string[]{
                "-p"
        };

            CommandLine cl = parser.parse(options, args);
            Assert.IsTrue(cl.hasOption("p"), "Confirm -p is set");
            Assert.IsFalse(cl.hasOption("attr"), "Confirm -attr is not set");
            Assert.IsTrue(cl.getArgs().Length == 0, "Confirm all arguments recognized");
        }
    }
}
