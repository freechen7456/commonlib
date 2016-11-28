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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.commons.cli;

namespace org.apache.commons.cli.bug
{
    /**
     * http://issues.apache.org/jira/browse/CLI-18
     */
    [TestClass]
    public class BugCLI18Test
    {
        [TestMethod]
        public void testCLI18()
        {
            Options options = new Options();
            options.addOption(new Option("a", "aaa", false, "aaaaaaa"));
            options.addOption(new Option(null, "bbb", false, "bbbbbbb dksh fkshd fkhs dkfhsdk fhskd hksdks dhfowehfsdhfkjshf skfhkshf sf jkshfk sfh skfh skf f"));
            options.addOption(new Option("c", null, false, "ccccccc"));

            HelpFormatter formatter = new HelpFormatter();
            StringWriter outWriter = new StringWriter();

            formatter.printHelp(outWriter, 80, "foobar", "dsfkfsh kdh hsd hsdh fkshdf ksdh fskdh fsdh fkshfk sfdkjhskjh fkjh fkjsh khsdkj hfskdhf skjdfh ksf khf s", options, 2, 2, "blort j jgj j jg jhghjghjgjhgjhg jgjhgj jhg jhg hjg jgjhghjg jhg hjg jhgjg jgjhghjg jg jgjhgjgjg jhg jhgjh" + '\r' + '\n' + "rarrr", true);
        }
    }
}
