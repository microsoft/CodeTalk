//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Serialization;
using static Microsoft.CodeTalk.Commands.CommandConstants;

namespace Microsoft.CodeTalk.Commands
{
    public class CommandKeyConfig
	{
		[XmlAttribute(AttributeName = "CommandKey")]
		public string CommandKeyString { get; set; }

		[XmlAttribute]
		public ModifierKeys CommandModifierKey { get; set; }

		[XmlIgnore]
		public Keys CommandKey { get; set; }

		public CommandKeyConfig() { }

		public CommandKeyConfig(string commandKey, string modifierKey)
		{
			this.CommandKey = CommandConstants.KeyNamesReverseMap[commandKey];
			this.CommandKeyString = commandKey;
			this.CommandModifierKey = (ModifierKeys)Enum.Parse(typeof(ModifierKeys), modifierKey);
		}

		public void ProcessAndValidate(string CommandType = "")
		{
			//Parsing CodeTalk Key
			Keys cKey;
			if (CommandConstants.KeyNamesReverseMap.TryGetValue(CommandKeyString, out cKey))
			{
				if (CommandConstants.KeyNamesMap.Keys.Contains(cKey))
				{
					CommandKey = cKey;
				}
				else
				{
					throw new Exception($"Invalid {CommandType} Command Key. The key is not supported.");
				}
			}
			else
			{
				throw new Exception($"Invalid {CommandType} Command Key. The key doesn't exist.");
			}
		}

		public void PrepareWrite(string CommandType = "")
		{
			var cKeyString = string.Empty;
			if (CommandConstants.KeyNamesMap.Keys.Contains(this.CommandKey))
			{
				if (CommandConstants.KeyNamesMap.TryGetValue(this.CommandKey, out cKeyString))
				{
					this.CommandKeyString = cKeyString;
				}
				else
				{
					throw new Exception($"Invalid {CommandType} Command Key. The key doesn't exist.");
				}
			}
			else
			{
				throw new Exception($"Invalid {CommandType} Command Key. The key is not supported.");
			}
		}
	}
}
