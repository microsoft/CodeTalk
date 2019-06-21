//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------


using Microsoft.CodeTalk.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Microsoft.CodeTalk.Commands.CommandConstants;

namespace Microsoft.CodeTalk
{
    public class CodeTalkConfig
	{
		[XmlElement(ElementName = "CodeTalkKey")]
		public string CodeTalkKeyString { get; set; }

		public ModifierKeys CodeTalkModifierKey { get; set; }

		[XmlElement(ElementName = "GetFunctionsCommand")]
		public CommandKeyConfig GetFunctionsCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "GetSummaryCommand")]
		public CommandKeyConfig GetSummaryCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "GetErrorsCommand")]
		public CommandKeyConfig GetErrorsCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "SkipCommentCommand")]
		public CommandKeyConfig SkipCommentCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "GetContextCommand")]
		public CommandKeyConfig GetContextCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "MoveToContextCommand")]
		public CommandKeyConfig MoveToContextCommandKeyConfig { get; set; }

		[XmlElement(ElementName = "CreateBreakPointCommand")]
		public CommandKeyConfig CreateBreakpointCommandKeyConfig { get; set; }

        [XmlElement(ElementName = "SetProfilepointsCommand")]
        public CommandKeyConfig SetProfilepointsCommandKeyConfig { get; set; }

        // XML Exclusions

        [XmlIgnore]
		public Keys CodeTalkKey { get; set; }

		List<CommandBase> Commands
		{
			get
			{
				return new List<CommandBase>
				{
					new GetFunctionsCommand(GetFunctionsCommandKeyConfig),
					new GetErrorsCommand(GetErrorsCommandKeyConfig),
					new GetSummaryCommand(GetSummaryCommandKeyConfig),
					new SkipCommentCommand(SkipCommentCommandKeyConfig),
					new GetContextCommand(GetContextCommandKeyConfig),
					new MoveToContextCommand(MoveToContextCommandKeyConfig),
					new CreateBreakpointCommand(CreateBreakpointCommandKeyConfig),
                    new SetProfilepointsCommand(SetProfilepointsCommandKeyConfig)
                };
			}
		}

		public void ProcessAndValidate()
		{
			//Parsing CodeTalk Key
			Keys cKey;
			if (CommandConstants.KeyNamesReverseMap.TryGetValue(CodeTalkKeyString, out cKey))
			{
				if (CommandConstants.SupportedCodeTalkKeys.Contains(cKey))
				{
					CodeTalkKey = cKey;
				}
				else
				{
					throw new Exception("Invalid CodeTalk key. Only the number keys and ~, -, and + are supported.");
				}
			}
			else
			{
				throw new Exception("Selected CodeTalk key doesn't exist.");
			}

			//Processing and validating commands
			GetFunctionsCommandKeyConfig.ProcessAndValidate("Get Functions");
			GetSummaryCommandKeyConfig.ProcessAndValidate("Get Summary");
			GetErrorsCommandKeyConfig.ProcessAndValidate("Get Errors");
			SkipCommentCommandKeyConfig.ProcessAndValidate("Skip comments");
			GetContextCommandKeyConfig.ProcessAndValidate("Get Context");
			MoveToContextCommandKeyConfig.ProcessAndValidate("Move To Context");
			CreateBreakpointCommandKeyConfig.ProcessAndValidate("Create Breakpoint");
            SetProfilepointsCommandKeyConfig.ProcessAndValidate("Set Profilepoints");


        }

		public void PrepareWrite()
		{
			var cKeyString = string.Empty;
			if (CommandConstants.SupportedCodeTalkKeys.Contains(this.CodeTalkKey))
			{
				if (CommandConstants.KeyNamesMap.TryGetValue(this.CodeTalkKey, out cKeyString))
				{
					this.CodeTalkKeyString = cKeyString;
				}
				else
				{
					throw new Exception("Invalid CodeTalk Key. The key doesn't exist.");
				}
			}
			else
			{
				throw new Exception("Invalid CodeTalk Key. The key is not supported.");
			}

			//Preparing for write : Commands
			GetFunctionsCommandKeyConfig.PrepareWrite("Get Functions");
			GetSummaryCommandKeyConfig.PrepareWrite("Get Summary");
			GetErrorsCommandKeyConfig.PrepareWrite("Get Errors");
			SkipCommentCommandKeyConfig.PrepareWrite("Skip comments");
			GetContextCommandKeyConfig.PrepareWrite("Get Context");
			MoveToContextCommandKeyConfig.PrepareWrite("Move To Context");
			CreateBreakpointCommandKeyConfig.PrepareWrite("Create Breakpoint");
            SetProfilepointsCommandKeyConfig.PrepareWrite("Set Profilepoints");

        }

		public List<CommandBase> GetCommands()
		{
			return this.Commands;
		}

		public static CodeTalkConfig LoadFromXml(string filePath)
		{
			filePath = Path.GetFullPath(filePath);
			CodeTalkConfig config = null;
			using (var fileStream = new FileStream(filePath, FileMode.Open))
			{
				var serializer = new XmlSerializer(typeof(CodeTalkConfig));
				config = serializer.Deserialize(fileStream) as CodeTalkConfig;
				fileStream.Close();
			}
			config.ProcessAndValidate();
			return config;
		}

		public void SaveAsXml(string filePath)
		{
			this.PrepareWrite();
			filePath = Path.GetFullPath(filePath);
			using (var fileStream = new FileStream(filePath, FileMode.Create))
			{
				var serializer = new XmlSerializer(typeof(CodeTalkConfig));
				serializer.Serialize(fileStream, this);
				fileStream.Close();
			}
		}

	}


}
