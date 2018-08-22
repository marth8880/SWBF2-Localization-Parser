﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZeroLocalizationTool.Modules;

namespace ZeroLocalizationTool
{
	class Program
	{
		static Dictionary<string, string[]> parsedArgs = new Dictionary<string, string[]>();
		static DataBase db = new DataBase();

		static void Main(string[] args)
		{
			void ArgError(string arg, string cmd)
			{
				Console.WriteLine(string.Format("Error: Invalid number of argument parameters specified for '{0}' \n", arg));
				Console.WriteLine("Usage:");
				Command_Usage(cmd);
				Exit(1);
			}

			// Parse command line arguments
			try
			{
				if (File.Exists(args[0]))
				{
					parsedArgs.Add("file", new string[] { args[0] });
				}
				else if (args[0] == "-h" || args[0] == "--help" || args[0] == "help")
				{
					if (args.Length == 1)
					{
						Command_Help();
						Exit();
					}
					else if (args.Length > 1)
					{
						Console.WriteLine("Usage:");
						switch (args[1])
						{
							case "-sv":
							case "--set-value":
								Command_Usage("set-value");
								break;

							case "-gv":
							case "--get-value":
								Command_Usage("get-value");
								break;
						}
						Exit();
					}
				}
				else
				{
					Console.WriteLine(string.Format("Error: File does not exist at path: '{0}'", args[0]));
					Exit(1);
				}

				for (int i = 1; i < args.Length; i++)
				{
					switch (args[i])
					{
						case "-sv":
						case "--set-value":
							string svKeyPath = args[i + 1];
							string svKeyValue = args[i + 2];

							if (svKeyPath.StartsWith("-")) ArgError(args[i], "set-value");
							if (svKeyValue.StartsWith("-")) ArgError(args[i], "set-value");

							parsedArgs.Add("set-value", new string[] { svKeyPath, svKeyValue });
							break;

						case "-gv":
						case "--get-value":
							string gvKeyPath = args[i + 1];

							if (gvKeyPath.StartsWith("-")) ArgError(args[i], "get-value");

							parsedArgs.Add("get-value", new string[] { gvKeyPath });
							break;
					}
				}
			}
			catch (IndexOutOfRangeException)
			{
				Console.WriteLine("Error: Invalid number of arguments.\n");
				Command_Help();
				Exit(1);
			}

			//parsedArgs["file"][0] = @"J:\BF2_ModTools\data_LCT\Common\Localize\english.cfg";
			//parsedArgs["file"][0] = @"J:\BF2_ModTools\data_TCW\data_TCW\Common\Localize\english.cfg";
			//parsedArgs["file"][0] = @"J:\BF2_ModTools\data_LCT\Common\Localize\english.cfg";

			if (parsedArgs.ContainsKey("file"))
			{
				try
				{
					db = LocalizationParser.ParseDataBase(parsedArgs["file"][0]);

					// Process arguments
					if (parsedArgs.ContainsKey("set-value"))
					{
						Command_SetValue();
					}

					if (parsedArgs.ContainsKey("get-value"))
					{
						Command_GetValue();
					}



					//Key testrootkey = db.GetKey("testrootkey");
					//testrootkey.SetValue("new test value");

					//db.WriteToFile(parsedArgs["file"][0]);
					//Console.WriteLine(StringExt.ConvertUnicodeListToString(db.GetKey("testrootkey").BinaryValues));

					//string testString = "\u0002";
					//Console.WriteLine(testString);

					//string testString = "\0\0TCW_Dev_Build_30813/01\r\nPROPERTY OF FRAYED WIRES STUDIOS\r\nDO NOT DISTRIBUTE";
					//List<string> testBinaryList = StringExt.ConvertStringToUnicodeList(testString);
					//foreach (string s in testBinaryList)
					//{
					//	Console.WriteLine(s);
					//}
					//Console.WriteLine("\n\nSize: " + testString.Length * 2);
					//Console.WriteLine("\n\n" + StringExt.ConvertUnicodeListToString(testBinaryList));

					//List<string> testList = new List<string>();
					//testList.Add("00000000450034007500F500440056006700F500240057009600C6004600F500");
					//testList.Add("33000300230013001300F20003001300D000A00005002500F400050054002500");
					//testList.Add("450095000200F400640002006400250014009500540044000200750094002500");
					//testList.Add("54003500020035004500550044009400F4003500D000A0004400F4000200E400");
					//testList.Add("F400450002004400940035004500250094002400550045005400");

					//Console.WriteLine(StringExt.ConvertUnicodeListToString(testList));
				}
				catch (System.Security.SecurityException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
				catch (NotSupportedException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
				catch (FileNotFoundException ex)
				{
					Console.WriteLine("Error: File not found at path: " + parsedArgs["file"][0]);
					Exit(3);
				}
				catch (UnauthorizedAccessException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
				catch (IOException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
				catch (ArgumentNullException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
				catch (ArgumentException ex)
				{
					Console.WriteLine(ex.Message);
					Command_Help();
					Exit(3);
				}
			}
			else
			{
				Console.WriteLine("Error: File not specified!\n");
				Command_Help();
				Exit(2);
			}
		}

		static void Exit(int exitCode = 0)
		{
			// Exit the application
			Console.WriteLine("\n" + "Press any key to exit.");
			Console.ReadKey();
			Environment.Exit(exitCode);
		}

		static void Command_Help()
		{
			Console.WriteLine("For help with a specific command, use 'help <command>', e.g. 'help -sv' \n");

			Console.WriteLine("Command list:\n");
			Command_Usage("help");
			//Command_Usage("file");
			Command_Usage("set-value");
			Command_Usage("get-value");
		}

		static void Command_Usage(string cmd)
		{
			switch (cmd)
			{
				case "help":
					Console.WriteLine("-h [command name]             Displays this command list, or the usage for the specified command. Alt: '--help', 'help'");
					break;

				//case "file":
				//	Console.WriteLine("-f <file path>                Specifies the localization file to open. Must be wrapped in quotes. Alt: '--file'");
				//	break;

				case "set-value":
					Console.WriteLine("-sv <key path> <key value>    Sets a new value for the specified key. Alt: '--set-value'");
					break;

				case "get-value":
					Console.WriteLine("-gv <key path>                Prints the value of the specified key. Alt: '--get-value'");
					break;
			}
		}

		static void Command_SetValue()
		{
			try
			{
				Key key = db.GetKey(parsedArgs["set-value"][0]);
				key.SetValue(parsedArgs["set-value"][1]);

				db.WriteToFile(parsedArgs["file"][0]);
			}
			catch (LocalizedKeyNotFoundException ex)
			{
				Console.WriteLine(ex.Message, ex.KeyPath);
				Exit(3);
			}
			catch (ObjectDisposedException ex)
			{
				Console.WriteLine(ex.Message, ex.ObjectName);
				throw;
			}
			catch (NotSupportedException ex)
			{
				Console.WriteLine(ex.Message);
				throw;
			}
			catch (FileNotFoundException)
			{
				Console.WriteLine("Error: File not found at path: " + parsedArgs["file"][0]);
				Exit(3);
			}
			catch (UnauthorizedAccessException ex)
			{
				Console.WriteLine(ex.Message);
				Command_Help();
				Exit(3);
			}
			catch (IOException ex)
			{
				Console.WriteLine(ex.Message);
				Command_Help();
				Exit(3);
			}
			catch (ArgumentNullException ex)
			{
				Console.WriteLine(ex.Message);
				Command_Help();
				Exit(3);
			}
			catch (ArgumentException ex)
			{
				Console.WriteLine(ex.Message);
				Command_Help();
				Exit(3);
			}
		}

		static void Command_GetValue()
		{
			try
			{
				Key key = db.GetKey(parsedArgs["get-value"][0]);

				Console.WriteLine(string.Format("Value of key at path '{0}': \n{1}", parsedArgs["get-value"][0], key.Value));
				Exit();
			}
			catch (LocalizedKeyNotFoundException ex)
			{
				Console.WriteLine(ex.Message);
				Exit(3);
			}
		}
	}
}
