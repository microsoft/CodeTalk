//------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Microsoft.CodeTalk.Commands
{
    public class CommandConstants
    {
        public enum ModifierKeys
        {
            Control,
            Shift,
            Insert
        }

        #region KeyboardProperties
        internal static List<Keys> SupportedCommandKeys = new List<Keys>{
            Keys.Left,
            Keys.Up,
            Keys.Right,
            Keys.Down,
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.A,
            Keys.B,
            Keys.C,
            Keys.D,
            Keys.E,
            Keys.F,
            Keys.G,
            Keys.H,
            Keys.I,
            Keys.J,
            Keys.K,
            Keys.L,
            Keys.M,
            Keys.N,
            Keys.O,
            Keys.P,
            Keys.Q,
            Keys.R,
            Keys.S,
            Keys.T,
            Keys.U,
            Keys.V,
            Keys.W,
            Keys.X,
            Keys.Y,
            Keys.Z,
            Keys.Oemplus,
            Keys.Oemcomma,
            Keys.OemMinus,
            Keys.OemPeriod,
            Keys.OemQuestion,
            Keys.OemBackslash,
        };

        internal static List<Keys> SupportedCodeTalkKeys = new List<Keys>{
            Keys.D0,
            Keys.D1,
            Keys.D2,
            Keys.D3,
            Keys.D4,
            Keys.D5,
            Keys.D6,
            Keys.D7,
            Keys.D8,
            Keys.D9,
            Keys.Oemplus,
            Keys.Oemcomma,
            Keys.OemMinus,
            Keys.OemPeriod,
            Keys.Oemtilde
        };

        internal static Dictionary<Keys, string> KeyNamesMap = new Dictionary<Keys, string>
        {
            {Keys.Space,"Space"},
            {Keys.PageUp,"PageUp"},
            {Keys.PageDown,"PageDown"},
            {Keys.End,"End"},
            {Keys.Home,"Home"},
            {Keys.Left,"Left"},
            {Keys.Up,"Up"},
            {Keys.Right,"Right"},
            {Keys.Down,"Down"},
            {Keys.Insert,"Insert"},
            {Keys.Delete,"Delete"},
            {Keys.D0,"0"},
            {Keys.D1,"1"},
            {Keys.D2,"2"},
            {Keys.D3,"3"},
            {Keys.D4,"4"},
            {Keys.D5,"5"},
            {Keys.D6,"6"},
            {Keys.D7,"7"},
            {Keys.D8,"8"},
            {Keys.D9,"9"},
            {Keys.A,"A"},
            {Keys.B,"B"},
            {Keys.C,"C"},
            {Keys.D,"D"},
            {Keys.E,"E"},
            {Keys.F,"F"},
            {Keys.G,"G"},
            {Keys.H,"H"},
            {Keys.I,"I"},
            {Keys.J,"J"},
            {Keys.K,"K"},
            {Keys.L,"L"},
            {Keys.M,"M"},
            {Keys.N,"N"},
            {Keys.O,"O"},
            {Keys.P,"P"},
            {Keys.Q,"Q"},
            {Keys.R,"R"},
            {Keys.S,"S"},
            {Keys.T,"T"},
            {Keys.U,"U"},
            {Keys.V,"V"},
            {Keys.W,"W"},
            {Keys.X,"X"},
            {Keys.Y,"Y"},
            {Keys.Z,"Z"},
            {Keys.NumPad0,"NumPad0"},
            {Keys.NumPad1,"NumPad1"},
            {Keys.NumPad2,"NumPad2"},
            {Keys.NumPad3,"NumPad3"},
            {Keys.NumPad4,"NumPad4"},
            {Keys.NumPad5,"NumPad5"},
            {Keys.NumPad6,"NumPad6"},
            {Keys.NumPad7,"NumPad7"},
            {Keys.NumPad8,"NumPad8"},
            {Keys.NumPad9,"NumPad9"},
            {Keys.Oemplus,"Plus"},
            {Keys.Oemcomma,"Comma"},
            {Keys.OemMinus,"Minus"},
            {Keys.OemPeriod,"Period"},
            {Keys.OemQuestion,"Question"},
            {Keys.Oemtilde,"Tilde"},
            {Keys.OemPipe,"Pipe"},
            {Keys.OemQuotes,"Quotes"},
            {Keys.OemBackslash,"Backslash"}
        };


        internal static Dictionary<String, Keys> KeyNamesReverseMap = KeyNamesMap.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
		#endregion

		//Default CodeTalkConfig
		internal static CodeTalkConfig DefualtCodeTalkConfig = new CodeTalkConfig()
		{
			CodeTalkKey = Keys.D1,
			CodeTalkKeyString = KeyNamesMap[Keys.D1],
			CodeTalkModifierKey = ModifierKeys.Control,
			GetFunctionsCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.F,
				CommandKeyString = KeyNamesMap[Keys.F],
				CommandModifierKey = ModifierKeys.Control
			},
			GetSummaryCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.M,
				CommandKeyString = KeyNamesMap[Keys.M],
				CommandModifierKey = ModifierKeys.Control
			},
			GetErrorsCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.E,
				CommandKeyString = KeyNamesMap[Keys.E],
				CommandModifierKey = ModifierKeys.Control
			},
			GetContextCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.G,
				CommandKeyString = KeyNamesMap[Keys.G],
				CommandModifierKey = ModifierKeys.Control
			},
			MoveToContextCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.J,
				CommandKeyString = KeyNamesMap[Keys.J],
				CommandModifierKey = ModifierKeys.Control
			},
			SkipCommentCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.C,
				CommandKeyString = KeyNamesMap[Keys.C],
				CommandModifierKey = ModifierKeys.Control
			},
			CreateBreakpointCommandKeyConfig = new CommandKeyConfig()
			{
				CommandKey = Keys.B,
				CommandKeyString = KeyNamesMap[Keys.B],
				CommandModifierKey = ModifierKeys.Control
			},
            SetProfilepointsCommandKeyConfig = new CommandKeyConfig()
            {
                CommandKey = Keys.K,
                CommandKeyString = KeyNamesMap[Keys.K],
                CommandModifierKey = ModifierKeys.Control
            }
        };
    }
}
