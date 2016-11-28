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
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace org.apache.commons.cli
{

    /** 
     * Test case for the HelpFormatter class.
     */
    [TestClass]
    public class HelpFormatterTest
    {
        private static readonly string EOL = Environment.NewLine;

        [TestMethod]
        public void testFindWrapPos()
        {
            HelpFormatter hf = new HelpFormatter();

            string text = "This is a test.";
            // text width should be max 8; the wrap position is 7
            Assert.AreEqual(7, hf.findWrapPos(text, 8, 0), "wrap position");

            // starting from 8 must give -1 - the wrap pos is after end
            Assert.AreEqual(-1, hf.findWrapPos(text, 8, 8), "wrap position 2");

            // words longer than the width are cut
            text = "aaaa aa";
            Assert.AreEqual(3, hf.findWrapPos(text, 3, 0), "wrap position 3");

            // last word length is equal to the width
            text = "aaaaaa aaaaaa";
            Assert.AreEqual(6, hf.findWrapPos(text, 6, 0), "wrap position 4");
            Assert.AreEqual(-1, hf.findWrapPos(text, 6, 7), "wrap position 4");

            text = "aaaaaa\n aaaaaa";
            Assert.AreEqual(7, hf.findWrapPos(text, 6, 0), "wrap position 5");

            text = "aaaaaa\t aaaaaa";
            Assert.AreEqual(7, hf.findWrapPos(text, 6, 0), "wrap position 6");
        }

        [TestMethod]
        public void testRenderWrappedTextWordCut()
        {
            int width = 7;
            int padding = 0;
            string text = "Thisisatest.";
            string expected = "Thisisa" + EOL +
                              "test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, text);
            Assert.AreEqual(expected, sb.ToString(), "cut and wrap");
        }

        [TestMethod]
        public void testRenderWrappedTextSingleLine()
        {
            // single line text
            int width = 12;
            int padding = 0;
            string text = "This is a test.";
            string expected = "This is a" + EOL +
                              "test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, text);
            Assert.AreEqual(expected, sb.ToString(), "single line text");
        }

        [TestMethod]
        public void testRenderWrappedTextSingleLinePadded()
        {
            // single line padded text
            int width = 12;
            int padding = 4;
            string text = "This is a test.";
            string expected = "This is a" + EOL +
                              "    test.";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, text);
            Assert.AreEqual(expected, sb.ToString(), "single line padded text");
        }

        [TestMethod]
        public void testRenderWrappedTextSingleLinePadded2()
        {
            // single line padded text 2
            int width = 53;
            int padding = 24;
            string text = "  -p,--period <PERIOD>  PERIOD is time duration of form " +
                          "DATE[-DATE] where DATE has form YYYY[MM[DD]]";
            string expected = "  -p,--period <PERIOD>  PERIOD is time duration of" + EOL +
                              "                        form DATE[-DATE] where DATE" + EOL +
                              "                        has form YYYY[MM[DD]]";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, text);
            Assert.AreEqual(expected, sb.ToString(), "single line padded text 2");
        }

        [TestMethod]
        public void testRenderWrappedTextMultiLine()
        {
            // multi line text
            int width = 16;
            int padding = 0;
            string expected = "aaaa aaaa aaaa" + EOL +
                          "aaaaaa" + EOL +
                          "aaaaa";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, expected);
            Assert.AreEqual(expected, sb.ToString(), "multi line text");
        }

        [TestMethod]
        public void testRenderWrappedTextMultiLinePadded()
        {
            // multi-line padded text
            int width = 16;
            int padding = 4;
            string text = "aaaa aaaa aaaa" + EOL +
                          "aaaaaa" + EOL +
                          "aaaaa";
            string expected = "aaaa aaaa aaaa" + EOL +
                              "    aaaaaa" + EOL +
                              "    aaaaa";

            StringBuilder sb = new StringBuilder();
            new HelpFormatter().renderWrappedText(sb, width, padding, text);
            Assert.AreEqual(expected, sb.ToString(),"multi-line padded text");
        }

        [TestMethod]
        public void testPrintOptions()
        {
            StringBuilder sb = new StringBuilder();
            HelpFormatter hf = new HelpFormatter();
            int leftPad = 1;
            int descPad = 3;
            string lpad = hf.createPadding(leftPad);
            string dpad = hf.createPadding(descPad);
            Options options = null;
            string expected = null;

            options = new Options().addOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
            expected = lpad + "-a" + dpad + "aaaa aaaa aaaa aaaa aaaa";
            hf.renderOptions(sb, 60, options, leftPad, descPad);
            Assert.AreEqual(expected, sb.ToString(), "simple non-wrapped option");

            int nextLineTabStop = leftPad + descPad + "-a".Length;
            expected = lpad + "-a" + dpad + "aaaa aaaa aaaa" + EOL +
                       hf.createPadding(nextLineTabStop) + "aaaa aaaa";
            sb.Length = 0;
            hf.renderOptions(sb, nextLineTabStop + 17, options, leftPad, descPad);
            Assert.AreEqual(expected, sb.ToString(), "simple wrapped option");


            options = new Options().addOption("a", "aaa", false, "dddd dddd dddd dddd");
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd dddd dddd";
            sb.Length = 0;
            hf.renderOptions(sb, 60, options, leftPad, descPad);
            Assert.AreEqual(expected, sb.ToString(), "long non-wrapped option");

            nextLineTabStop = leftPad + descPad + "-a,--aaa".Length;
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + EOL +
                       hf.createPadding(nextLineTabStop) + "dddd dddd";
            sb.Length = 0;
            hf.renderOptions(sb, 25, options, leftPad, descPad);
            Assert.AreEqual(expected, sb.ToString(), "long wrapped option");

            options = new Options().
                    addOption("a", "aaa", false, "dddd dddd dddd dddd").
                    addOption("b", false, "feeee eeee eeee eeee");
            expected = lpad + "-a,--aaa" + dpad + "dddd dddd" + EOL +
                       hf.createPadding(nextLineTabStop) + "dddd dddd" + EOL +
                       lpad + "-b      " + dpad + "feeee eeee" + EOL +
                       hf.createPadding(nextLineTabStop) + "eeee eeee";
            sb.Length = 0;
            hf.renderOptions(sb, 25, options, leftPad, descPad);
            Assert.AreEqual(expected, sb.ToString(), "multiple wrapped options");
        }

        [TestMethod]
        public void testPrintHelpWithEmptySyntax()
        {
            HelpFormatter formatter = new HelpFormatter();
            try
            {
                formatter.printHelp(null, new Options());
                Assert.Fail("null command line syntax should be rejected");
            }
            catch (ArgumentException e)
            {
                // expected
            }

            try
            {
                formatter.printHelp("", new Options());
                Assert.Fail("empty command line syntax should be rejected");
            }
            catch (ArgumentException e)
            {
                // expected
            }
        }

        [TestMethod]
        public void testAutomaticUsage()
        {
            HelpFormatter hf = new HelpFormatter();
            Options options = null;
            string expected = "usage: app [-a]";
            MemoryStream outStream = new MemoryStream();
            StreamWriter pw = new StreamWriter(outStream);

            options = new Options().addOption("a", false, "aaaa aaaa aaaa aaaa aaaa");
            hf.printUsage(pw, 60, "app", options);
            pw.Flush();
            Assert.AreEqual(expected, Encoding.Default.GetString(outStream.ToArray()).Trim(), "simple auto usage");
            outStream.Position = 0;
            outStream.SetLength(0);

            expected = "usage: app [-a] [-b]";
            options = new Options().addOption("a", false, "aaaa aaaa aaaa aaaa aaaa")
                    .addOption("b", false, "bbb");
            hf.printUsage(pw, 60, "app", options);
            pw.Flush();
            Assert.AreEqual(expected, Encoding.Default.GetString(outStream.ToArray()).Trim(), "simple auto usage");
            outStream.Position = 0;
            outStream.SetLength(0);
        }

        // This test ensures the options are properly sorted
        // See https://issues.apache.org/jira/browse/CLI-131
        [TestMethod]
        public void testPrintUsage()
        {
            Option optionA = new Option("a", "first");
            Option optionB = new Option("b", "second");
            Option optionC = new Option("c", "third");
            Options opts = new Options();
            opts.addOption(optionA);
            opts.addOption(optionB);
            opts.addOption(optionC);
            HelpFormatter helpFormatter = new HelpFormatter();
            MemoryStream bytesOut = new MemoryStream();
            StreamWriter printWriter = new StreamWriter(bytesOut);
            helpFormatter.printUsage(printWriter, 80, "app", opts);
            printWriter.Close();
            Assert.AreEqual("usage: app [-a] [-b] [-c]" + EOL, Encoding.Default.GetString(bytesOut.ToArray()).Trim(" ".ToCharArray()));
        }

        // uses the test for CLI-131 to implement CLI-155
        [TestMethod]
        public void testPrintSortedUsage()
        {
            Options opts = new Options();
            opts.addOption(new Option("a", "first"));
            opts.addOption(new Option("b", "second"));
            opts.addOption(new Option("c", "third"));

            HelpFormatter helpFormatter = new HelpFormatter();
            helpFormatter.setOptionComparator(new OptionComparer());

            StringWriter outWriter = new StringWriter();
            helpFormatter.printUsage(outWriter, 80, "app", opts);

            Assert.AreEqual("usage: app [-c] [-b] [-a]" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testPrintSortedUsageWithNullComparator()
        {
            Options opts = new Options();
            opts.addOption(new Option("c", "first"));
            opts.addOption(new Option("b", "second"));
            opts.addOption(new Option("a", "third"));

            HelpFormatter helpFormatter = new HelpFormatter();
            helpFormatter.setOptionComparator(null);

            StringWriter outWriter = new StringWriter();
            helpFormatter.printUsage(outWriter, 80, "app", opts);

            Assert.AreEqual("usage: app [-c] [-b] [-a]" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testPrintOptionGroupUsage()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(Option.builder("a").build());
            group.addOption(Option.builder("b").build());
            group.addOption(Option.builder("c").build());

            Options options = new Options();
            options.addOptionGroup(group);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.printUsage(outWriter, 80, "app", options);

            Assert.AreEqual("usage: app [-a | -b | -c]" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testPrintRequiredOptionGroupUsage()
        {
            OptionGroup group = new OptionGroup();
            group.addOption(Option.builder("a").build());
            group.addOption(Option.builder("b").build());
            group.addOption(Option.builder("c").build());
            group.setRequired(true);

            Options options = new Options();
            options.addOptionGroup(group);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.printUsage(outWriter, 80, "app", options);

            Assert.AreEqual("usage: app -a | -b | -c" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testPrintOptionWithEmptyArgNameUsage()
        {
            Option option = new Option("f", true, null);
            option.setArgName("");
            option.setRequired(true);

            Options options = new Options();
            options.addOption(option);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.printUsage(outWriter, 80, "app", options);

            Assert.AreEqual("usage: app -f" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testDefaultArgName()
        {
            Option option = Option.builder("f").hasArg().required(true).build();

            Options options = new Options();
            options.addOption(option);

            StringWriter outWriter = new StringWriter();

            HelpFormatter formatter = new HelpFormatter();
            formatter.setArgName("argument");
            formatter.printUsage(outWriter, 80, "app", options);

            Assert.AreEqual("usage: app -f <argument>" + EOL, outWriter.ToString());
        }

        [TestMethod]
        public void testRtrim()
        {
            HelpFormatter formatter = new HelpFormatter();

            Assert.AreEqual(null, formatter.rtrim(null));
            Assert.AreEqual("", formatter.rtrim(""));
            Assert.AreEqual("  foo", formatter.rtrim("  foo  "));
        }

        [TestMethod]
        public void testAccessors()
        {
            HelpFormatter formatter = new HelpFormatter();

            formatter.setArgName("argname");
            Assert.AreEqual("argname", formatter.getArgName(), "arg name");

            formatter.setDescPadding(3);
            Assert.AreEqual(3, formatter.getDescPadding(), "desc padding");

            formatter.setLeftPadding(7);
            Assert.AreEqual(7, formatter.getLeftPadding(), "left padding");

            formatter.setLongOptPrefix("~~");
            Assert.AreEqual("~~", formatter.getLongOptPrefix(), "long opt prefix");

            formatter.setNewLine("\n");
            Assert.AreEqual("\n", formatter.getNewLine(), "new line");

            formatter.setOptPrefix("~");
            Assert.AreEqual("~", formatter.getOptPrefix(), "opt prefix");

            formatter.setSyntaxPrefix("-> ");
            Assert.AreEqual("-> ", formatter.getSyntaxPrefix(), "syntax prefix");

            formatter.setWidth(80);
            Assert.AreEqual(80, formatter.getWidth(), "width");
        }

        [TestMethod]
        public void testHeaderStartingWithLineSeparator()
        {
            // related to Bugzilla #21215
            Options options = new Options();
            HelpFormatter formatter = new HelpFormatter();
            string header = EOL + "Header";
            string footer = "Footer";
            StringWriter outWriter = new StringWriter();
            formatter.printHelp(outWriter, 80, "foobar", header, options, 2, 2, footer, true);
            Assert.AreEqual(
                    "usage: foobar" + EOL +
                    "" + EOL +
                    "Header" + EOL +
                    "" + EOL +
                    "Footer" + EOL
                    , outWriter.ToString());
        }

        [TestMethod]
        public void testIndentedHeaderAndFooter()
        {
            // related to CLI-207
            Options options = new Options();
            HelpFormatter formatter = new HelpFormatter();
            string header = "  Header1\n  Header2";
            string footer = "  Footer1\n  Footer2";
            StringWriter outWriter = new StringWriter();
            formatter.printHelp(outWriter, 80, "foobar", header, options, 2, 2, footer, true);

            Assert.AreEqual(
                    "usage: foobar" + EOL +
                    "  Header1" + EOL +
                    "  Header2" + EOL +
                    "" + EOL +
                    "  Footer1" + EOL +
                    "  Footer2" + EOL
                    , outWriter.ToString());
        }

        [TestMethod]
        public void testOptionWithoutShortFormat()
        {
            // related to Bugzilla #19383 (CLI-67)
            Options options = new Options();
            options.addOption(new Option("a", "aaa", false, "aaaaaaa"));
            options.addOption(new Option(null, "bbb", false, "bbbbbbb"));
            options.addOption(new Option("c", null, false, "ccccccc"));

            HelpFormatter formatter = new HelpFormatter();
            StringWriter outWriter = new StringWriter();
            formatter.printHelp(outWriter, 80, "foobar", "", options, 2, 2, "", true);
            Assert.AreEqual(
                    "usage: foobar [-a] [--bbb] [-c]" + EOL +
                    "  -a,--aaa  aaaaaaa" + EOL +
                    "     --bbb  bbbbbbb" + EOL +
                    "  -c        ccccccc" + EOL
                    , outWriter.ToString());
        }

        [TestMethod]
        public void testOptionWithoutShortFormat2()
        {
            // related to Bugzilla #27635 (CLI-26)
            Option help = new Option("h", "help", false, "print this message");
            Option version = new Option("v", "version", false, "print version information");
            Option newRun = new Option("n", "new", false, "Create NLT cache entries only for new items");
            Option trackerRun = new Option("t", "tracker", false, "Create NLT cache entries only for tracker items");

            Option timeLimit = Option.builder("l")
                                     .longOpt("limit")
                                     .hasArg()
                                     .valueSeparator()
                                     .desc("Set time limit for execution, in mintues")
                                     .build();

            Option age = Option.builder("a").longOpt("age")
                                            .hasArg()
                                            .valueSeparator()
                                            .desc("Age (in days) of cache item before being recomputed")
                                            .build();

            Option server = Option.builder("s").longOpt("server")
                                               .hasArg()
                                               .valueSeparator()
                                               .desc("The NLT server address")
                                               .build();

            Option numResults = Option.builder("r").longOpt("results")
                                                   .hasArg()
                                                   .valueSeparator()
                                                   .desc("Number of results per item")
                                                   .build();

            Option configFile = Option.builder().longOpt("config")
                                                .hasArg()
                                                .valueSeparator()
                                                .desc("Use the specified configuration file")
                                                .build();

            Options mOptions = new Options();
            mOptions.addOption(help);
            mOptions.addOption(version);
            mOptions.addOption(newRun);
            mOptions.addOption(trackerRun);
            mOptions.addOption(timeLimit);
            mOptions.addOption(age);
            mOptions.addOption(server);
            mOptions.addOption(numResults);
            mOptions.addOption(configFile);

            HelpFormatter formatter = new HelpFormatter();
            string EOL = Environment.NewLine;
            StringWriter outWriter = new StringWriter();
            formatter.printHelp(outWriter, 80, "commandline", "header", mOptions, 2, 2, "footer", true);
            Assert.AreEqual(
                    "usage: commandline [-a <arg>] [--config <arg>] [-h] [-l <arg>] [-n] [-r <arg>]" + EOL +
                    "       [-s <arg>] [-t] [-v]" + EOL +
                    "header" + EOL +
                    "  -a,--age <arg>      Age (in days) of cache item before being recomputed" + EOL +
                    "     --config <arg>   Use the specified configuration file" + EOL +
                    "  -h,--help           print this message" + EOL +
                    "  -l,--limit <arg>    Set time limit for execution, in mintues" + EOL +
                    "  -n,--new            Create NLT cache entries only for new items" + EOL +
                    "  -r,--results <arg>  Number of results per item" + EOL +
                    "  -s,--server <arg>   The NLT server address" + EOL +
                    "  -t,--tracker        Create NLT cache entries only for tracker items" + EOL +
                    "  -v,--version        print version information" + EOL +
                    "footer" + EOL
                    , outWriter.ToString());
        }

        [TestMethod]
        public void testHelpWithLongOptSeparator()
        {
            Options options = new Options();
            options.addOption("f", true, "the file");
            options.addOption(Option.builder("s").longOpt("size").desc("the size").hasArg().argName("SIZE").build());
            options.addOption(Option.builder().longOpt("age").desc("the age").hasArg().build());

            HelpFormatter formatter = new HelpFormatter();
            Assert.AreEqual(HelpFormatter.DEFAULT_LONG_OPT_SEPARATOR, formatter.getLongOptSeparator());
            formatter.setLongOptSeparator("=");
            Assert.AreEqual("=", formatter.getLongOptSeparator());

            StringWriter outWriter = new StringWriter();

            formatter.printHelp(outWriter, 80, "create", "header", options, 2, 2, "footer");

            Assert.AreEqual(
                    "usage: create" + EOL +
                    "header" + EOL +
                    "     --age=<arg>    the age" + EOL +
                    "  -f <arg>          the file" + EOL +
                    "  -s,--size=<SIZE>  the size" + EOL +
                    "footer" + EOL,
                    outWriter.ToString());
        }

        [TestMethod]
        public void testUsageWithLongOptSeparator()
        {
            Options options = new Options();
            options.addOption("f", true, "the file");
            options.addOption(Option.builder("s").longOpt("size").desc("the size").hasArg().argName("SIZE").build());
            options.addOption(Option.builder().longOpt("age").desc("the age").hasArg().build());

            HelpFormatter formatter = new HelpFormatter();
            formatter.setLongOptSeparator("=");

            StringWriter outWriter = new StringWriter();

            formatter.printUsage(outWriter, 80, "create", options);

            Assert.AreEqual("usage: create [--age=<arg>] [-f <arg>] [-s <SIZE>]", outWriter.ToString().Trim());
        }
        class OptionComparer : System.Collections.Generic.IComparer<Option>
        {
            int System.Collections.Generic.IComparer<Option>.Compare(Option opt1, Option opt2)
            {
                // reverses the functionality of the default comparator
                return string.Compare(opt2.getKey(), opt1.getKey());
            }
        }
    }
}
