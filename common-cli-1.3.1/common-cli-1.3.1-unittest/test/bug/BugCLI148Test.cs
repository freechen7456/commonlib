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
using org.apache.commons.cli;

namespace org.apache.commons.cli.bug
{
    /**
     * http://issues.apache.org/jira/browse/CLI-148
     */
    [TestClass]
    public class BugCLI148Test
    {
        private Options options;

        [TestInitialize]
        public void setUp()
        {
            options = new Options();
            options.addOption(OptionBuilder.hasArg().create('t'));
            options.addOption(OptionBuilder.hasArg().create('s'));
        }

        [TestMethod]
        public void testWorkaround1()
        {
            CommandLineParser parser = new PosixParser();
            String[] args = new String[] { "-t-something" };

            CommandLine commandLine = parser.parse(options, args);
            Assert.AreEqual("-something", commandLine.getOptionValue('t'));
        }

        [TestMethod]
        public void testWorkaround2()
        {
            CommandLineParser parser = new PosixParser();
            String[] args = new String[] { "-t", "\"-something\"" };

            CommandLine commandLine = parser.parse(options, args);
            Assert.AreEqual("-something", commandLine.getOptionValue('t'));
        }
    }
}
