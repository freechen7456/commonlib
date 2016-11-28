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
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using org.apache.commons.cli;
namespace org.apache.commons.cli.bug
{
    [Obsolete]
    [TestClass]
    public class BugsTest
    {
        [TestMethod]
        public void test11457()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.withLongOpt("verbose").create());
            String[] args = new String[] { "--verbose" };

            CommandLineParser parser = new PosixParser();

            CommandLine cmd = parser.parse(options, args);
            Assert.IsTrue(cmd.hasOption("verbose"));
        }

        [TestMethod]
        public void test11458()
        {
            Options options = new Options();
            options.addOption(OptionBuilder.withValueSeparator('=').hasArgs().create('D'));
            options.addOption(OptionBuilder.withValueSeparator(':').hasArgs().create('p'));
            String[] args = new String[] { "-DJAVA_HOME=/opt/java", "-pfile1:file2:file3" };

            CommandLineParser parser = new PosixParser();

            CommandLine cmd = parser.parse(options, args);

            String[] values = cmd.getOptionValues('D');

            Assert.AreEqual(values[0], "JAVA_HOME");
            Assert.AreEqual(values[1], "/opt/java");

            values = cmd.getOptionValues('p');

            Assert.AreEqual(values[0], "file1");
            Assert.AreEqual(values[1], "file2");
            Assert.AreEqual(values[2], "file3");

            IIterator<Option> iter = cmd.iterator();
            while (iter.hasNext())
            {
                Option opt = iter.next();
                switch (opt.getId())
                {
                    case 'D':
                        Assert.AreEqual(opt.getValue(0), "JAVA_HOME");
                        Assert.AreEqual(opt.getValue(1), "/opt/java");
                        break;
                    case 'p':
                        Assert.AreEqual(opt.getValue(0), "file1");
                        Assert.AreEqual(opt.getValue(1), "file2");
                        Assert.AreEqual(opt.getValue(2), "file3");
                        break;
                    default:
                        Assert.Fail("-D option not found");
                        break;
                }
            }
        }

        [TestMethod]
        public void test11680()
        {
            Options options = new Options();
            options.addOption("f", true, "foobar");
            options.addOption("m", true, "missing");
            String[] args = new String[] { "-f", "foo" };

            CommandLineParser parser = new PosixParser();

            CommandLine cmd = parser.parse(options, args);

            cmd.getOptionValue("f", "default f");
            cmd.getOptionValue("m", "default m");
        }

        [TestMethod]
        public void test11456()
        {
            // Posix 
            Options options = new Options();
            options.addOption(OptionBuilder.hasOptionalArg().create('a'));
            options.addOption(OptionBuilder.hasArg().create('b'));
            String[] args = new String[] { "-a", "-bvalue" };

            CommandLineParser parser = new PosixParser();

            CommandLine cmd = parser.parse(options, args);
            Assert.AreEqual(cmd.getOptionValue('b'), "value");

            // GNU
            options = new Options();
            options.addOption(OptionBuilder.hasOptionalArg().create('a'));
            options.addOption(OptionBuilder.hasArg().create('b'));
            args = new String[] { "-a", "-b", "value" };

            parser = new GnuParser();

            cmd = parser.parse(options, args);
            Assert.AreEqual(cmd.getOptionValue('b'), "value");
        }

        [TestMethod]
        public void test12210()
        {
            // create the main options object which will handle the first parameter
            Options mainOptions = new Options();
            // There can be 2 main exclusive options:  -exec|-rep

            // Therefore, place them in an option group

            String[] argv = new String[] { "-exec", "-exec_opt1", "-exec_opt2" };
            OptionGroup grp = new OptionGroup();

            grp.addOption(new Option("exec", false, "description for this option"));

            grp.addOption(new Option("rep", false, "description for this option"));

            mainOptions.addOptionGroup(grp);

            // for the exec option, there are 2 options...
            Options execOptions = new Options();
            execOptions.addOption("exec_opt1", false, " desc");
            execOptions.addOption("exec_opt2", false, " desc");

            // similarly, for rep there are 2 options...
            Options repOptions = new Options();
            repOptions.addOption("repopto", false, "desc");
            repOptions.addOption("repoptt", false, "desc");

            // create the parser
            GnuParser parser = new GnuParser();

            // finally, parse the arguments:

            // first parse the main options to see what the user has specified
            // We set stopAtNonOption to true so it does not touch the remaining
            // options
            CommandLine cmd = parser.parse(mainOptions, argv, true);
            // get the remaining options...
            argv = cmd.getArgs();

            if (cmd.hasOption("exec"))
            {
                cmd = parser.parse(execOptions, argv, false);
                // process the exec_op1 and exec_opt2...
                Assert.IsTrue(cmd.hasOption("exec_opt1"));
                Assert.IsTrue(cmd.hasOption("exec_opt2"));
            }
            else if (cmd.hasOption("rep"))
            {
                cmd = parser.parse(repOptions, argv, false);
                // process the rep_op1 and rep_opt2...
            }
            else
            {
                Assert.Fail("exec option not found");
            }
        }

        [TestMethod]
        public void test13425()
        {
            Options options = new Options();
            Option oldpass = OptionBuilder.withLongOpt("old-password")
                .withDescription("Use this option to specify the old password")
                .hasArg()
                .create('o');
            Option newpass = OptionBuilder.withLongOpt("new-password")
                .withDescription("Use this option to specify the new password")
                .hasArg()
                .create('n');

            String[] args = { 
            "-o", 
            "-n", 
            "newpassword" 
        };

            options.addOption(oldpass);
            options.addOption(newpass);

            Parser parser = new PosixParser();

            try
            {
                parser.parse(options, args);
                Assert.Fail("MissingArgumentException not caught.");
            }
            catch (MissingArgumentException expected)
            {
            }
        }

        [TestMethod]
        public void test13666()
        {
            Options options = new Options();
            Option dir = OptionBuilder.withDescription("dir").hasArg().create('d');
            options.addOption(dir);

            TextWriter oldSystemOut = Console.Out;
            try
            {
                MemoryStream bytes = new MemoryStream();
                TextWriter print = new StreamWriter(bytes);

                // capture this platform's eol symbol
                print.WriteLine();
                print.Flush();
                String eol = System.Text.Encoding.Default.GetString(bytes.ToArray());
                bytes.Position = 0;
                bytes.SetLength(0);

                Console.SetOut(new StreamWriter(bytes));

                HelpFormatter formatter = new HelpFormatter();
                formatter.printHelp("dir", options);

                Assert.AreEqual("usage: dir" + eol + " -d <arg>   dir" + eol, System.Text.Encoding.Default.GetString(bytes.ToArray()));
            }
            finally
            {
                Console.SetOut(oldSystemOut);
            }
        }

        [TestMethod]
        public void test13935()
        {
            OptionGroup directions = new OptionGroup();

            Option left = new Option("l", "left", false, "go left");
            Option right = new Option("r", "right", false, "go right");
            Option straight = new Option("s", "straight", false, "go straight");
            Option forward = new Option("f", "forward", false, "go forward");
            forward.setRequired(true);

            directions.addOption(left);
            directions.addOption(right);
            directions.setRequired(true);

            Options opts = new Options();
            opts.addOptionGroup(directions);
            opts.addOption(straight);

            CommandLineParser parser = new PosixParser();

            String[] args = new String[] { };
            try
            {
                parser.parse(opts, args);
                Assert.Fail("Expected ParseException");
            }
            catch (ParseException expected)
            {
            }

            args = new String[] { "-s" };
            try
            {
                parser.parse(opts, args);
                Assert.Fail("Expected ParseException");
            }
            catch (ParseException expected)
            {
            }

            args = new String[] { "-s", "-l" };
            CommandLine line = parser.parse(opts, args);
            Assert.IsNotNull(line);

            opts.addOption(forward);
            args = new String[] { "-s", "-l", "-f" };
            line = parser.parse(opts, args);
            Assert.IsNotNull(line);
        }

        [TestMethod]
        public void test14786()
        {
            Option o = OptionBuilder.isRequired().withDescription("test").create("test");
            Options opts = new Options();
            opts.addOption(o);
            opts.addOption(o);

            CommandLineParser parser = new GnuParser();

            String[] args = new String[] { "-test" };

            CommandLine line = parser.parse(opts, args);
            Assert.IsTrue(line.hasOption("test"));
        }

        [TestMethod]
        public void test15046()
        {
            CommandLineParser parser = new PosixParser();
            String[] CLI_ARGS = new String[] { "-z", "c" };

            Options options = new Options();
            options.addOption(new Option("z", "timezone", true, "affected option"));

            parser.parse(options, CLI_ARGS);

            //now add conflicting option
            options.addOption("c", "conflict", true, "conflict option");
            CommandLine line = parser.parse(options, CLI_ARGS);
            Assert.AreEqual(line.getOptionValue('z'), "c");
            Assert.IsTrue(!line.hasOption("c"));
        }

        [TestMethod]
        public void test15648()
        {
            CommandLineParser parser = new PosixParser();
            String[] args = new String[] { "-m", "\"Two Words\"" };
            Option m = OptionBuilder.hasArgs().create("m");
            Options options = new Options();
            options.addOption(m);
            CommandLine line = parser.parse(options, args);
            Assert.AreEqual("Two Words", line.getOptionValue("m"));
        }

        [TestMethod]
        public void test31148()
        {
            Option multiArgOption = new Option("o", "option with multiple args");
            multiArgOption.setArgs(1);

            Options options = new Options();
            options.addOption(multiArgOption);

            Parser parser = new PosixParser();
            String[] args = new String[] { };
            Dictionary<string, string> props = new Dictionary<string, string>();
            props.Add("o", "ovalue");
            CommandLine cl = parser.parse(options, args, props);

            Assert.IsTrue(cl.hasOption('o'));
            Assert.AreEqual("ovalue", cl.getOptionValue('o'));
        }

    }
}
