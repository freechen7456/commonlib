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
    /** 
     * Test case for the PatternOptionBuilder class.
     */
    [TestClass]
    public class PatternOptionBuilderTest
    {
        [TestMethod]
        public void testSimplePattern()
        {
            Options options = PatternOptionBuilder.parsePattern("a:b@cde>f+n%t/m*z#");
            string[] args = new string[] { "-c", "-a", "foo", "-b", "System.Collections.ArrayList", "-e", "build.xml", "-f", "System.Globalization.Calendar", "-n", "4.5", "-t", "http://commons.apache.org", "-z", "Thu Jun 06 17:48:57 EDT 2002", "-m", "test*" };

            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, args);

            Assert.AreEqual("foo", line.getOptionValue("a"), "flag a");
            Assert.AreEqual("foo", line.getOptionObject("a"), "string flag a");
            CollectionAssert.AreEqual(new System.Collections.ArrayList(), (System.Collections.ArrayList)line.getOptionObject("b"), "object flag b");
            Assert.IsTrue(line.hasOption("c"), "bool true flag c");
            Assert.IsFalse(line.hasOption("d"), "bool false flag d");
            Assert.AreEqual(new System.IO.FileInfo("build.xml").ToString(), line.getOptionObject("e").ToString(), "file flag e");
            Assert.AreEqual(typeof(System.Globalization.Calendar), line.getOptionObject("f"), "class flag f");
            Assert.AreEqual((double)(4.5), line.getOptionObject("n"), "number flag n");
            Assert.AreEqual(new Uri("http://commons.apache.org"), line.getOptionObject("t"), "url flag t");

            // tests the char methods of CommandLine that delegate to the string methods
            Assert.AreEqual("foo", line.getOptionValue('a'), "flag a");
            Assert.AreEqual("foo", line.getOptionObject('a'), "string flag a");
            CollectionAssert.AreEqual(new System.Collections.ArrayList(), (System.Collections.ArrayList)line.getOptionObject('b'), "object flag b");
            Assert.IsTrue(line.hasOption('c'), "bool true flag c");
            Assert.IsFalse(line.hasOption('d'), "bool false flag d");
            Assert.AreEqual(new System.IO.FileInfo("build.xml").ToString(), line.getOptionObject('e').ToString(), "file flag e");
            Assert.AreEqual(typeof(System.Globalization.Calendar), line.getOptionObject('f'), "class flag f");
            Assert.AreEqual((double)(4.5), line.getOptionObject('n'), "number flag n");
            Assert.AreEqual(new Uri("http://commons.apache.org"), line.getOptionObject('t'), "url flag t");

            // FILES NOT SUPPORTED YET
            try
            {
                Assert.AreEqual(new System.IO.FileInfo[0], line.getOptionObject('m'), "files flag m");
                Assert.Fail("Multiple files are not supported yet, should have failed");
            }
            catch (UnrecognizedOptionException uoe)
            {
                // expected
            }
            catch (NotImplementedException uoe)
            {
                // expected
            }

            // DATES NOT SUPPORTED YET
            try
            {
                Assert.AreEqual(new DateTime(1023400137276L), line.getOptionObject('z'), "date flag z");
                Assert.Fail("Date is not supported yet, should have failed");
            }
            catch (UnrecognizedOptionException uoe)
            {
                // expected
            }
            catch (NotImplementedException uoe)
            {
                // expected
            }
        }

        [TestMethod]
        public void testEmptyPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("");
            Assert.IsTrue(options.getOptions().isEmpty());
        }

        [TestMethod]
        public void testUntypedPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("abc");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-abc" });

            Assert.IsTrue(line.hasOption('a'));
            Assert.IsNull(line.getOptionObject('a'), "value a");
            Assert.IsTrue(line.hasOption('b'));
            Assert.IsNull(line.getOptionObject('b'), "value b");
            Assert.IsTrue(line.hasOption('c'));
            Assert.IsNull(line.getOptionObject('c'), "value c");
        }

        [TestMethod]
        public void testNumberPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("n%d%x%");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-n", "1", "-d", "2.1", "-x", "3,5" });

            Assert.AreEqual(typeof(long), line.getOptionObject("n").GetType(), "n object class");
            Assert.AreEqual((long)(1), line.getOptionObject("n"), "n value");

            Assert.AreEqual(typeof(double), line.getOptionObject("d").GetType(), "d object class");
            Assert.AreEqual((double)(2.1), line.getOptionObject("d"), "d value");

            Assert.IsNull(line.getOptionObject("x"), "x object");
        }

        [TestMethod]
        public void testClassPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("c+d+");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-c", "System.Globalization.Calendar", "-d", "System.DateTime" });

            Assert.AreEqual(typeof(System.Globalization.Calendar), line.getOptionObject("c"), "c value");
            Assert.AreEqual(typeof(DateTime), line.getOptionObject("d"), "d value");
        }

        [TestMethod]
        public void testObjectPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("o@i@n@");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-o", "System.String", "-i", "System.Globalization.Calendar", "-n", "System.DateTime" });

            Assert.AreEqual("", line.getOptionObject("o"), "o value");
            Assert.IsNull(line.getOptionObject("i"), "i value");
            Assert.IsNull(line.getOptionObject("n"), "n value");
        }

        [TestMethod]
        public void testURLPattern()
        {
            Options options = PatternOptionBuilder.parsePattern("u/v/");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-u", "http://commons.apache.org", "-v", "foo://commons.apache.org" });

            Assert.AreEqual(new Uri("http://commons.apache.org"), line.getOptionObject("u"), "u value");
            Assert.AreEqual(new Uri("foo://commons.apache.org"), line.getOptionObject("v"), "v value");
        }

        [TestMethod]
        public void testExistingFilePattern()
        {
            Options options = PatternOptionBuilder.parsePattern("f<");
            CommandLineParser parser = new PosixParser();
            CommandLine line = parser.parse(options, new string[] { "-f", "test.properties" });

            Assert.AreEqual(new System.IO.FileInfo("test.properties").ToString(), line.getOptionObject("f").ToString(), "f value");

            // todo test if an error is returned if the file doesn't exists (when it's implemented)
        }

        [TestMethod]
        public void testRequiredOption()
        {
            Options options = PatternOptionBuilder.parsePattern("!n%m%");
            CommandLineParser parser = new PosixParser();

            try
            {
                parser.parse(options, new string[] { "" });
                Assert.Fail("MissingOptionException wasn't thrown");
            }
            catch (MissingOptionException e)
            {
                Assert.AreEqual(1, e.getMissingOptions().Count);
                Assert.IsTrue(e.getMissingOptions().Contains("n"));
            }
        }
    }
}
