//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from D:/workspaces/compilers/cspg/Grammer.ParserGenerator/Interperter.Kaleidoscope/Kaleidoscope.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using System;
using System.IO;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public partial class KaleidoscopeLexer : Lexer {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		T__0=1, T__1=2, T__2=3, T__3=4, T__4=5, T__5=6, WHITESPACE=7, DEF=8, EXTERN=9, 
		NUMBER=10, IDENTIFIER=11, OP=12;
	public static string[] channelNames = {
		"DEFAULT_TOKEN_CHANNEL", "HIDDEN"
	};

	public static string[] modeNames = {
		"DEFAULT_MODE"
	};

	public static readonly string[] ruleNames = {
		"T__0", "T__1", "T__2", "T__3", "T__4", "T__5", "WHITESPACE", "DEF", "EXTERN", 
		"NUMBER", "IDENTIFIER", "ADD", "SUB", "MUL", "LESS", "OP"
	};


	public KaleidoscopeLexer(ICharStream input)
	: this(input, Console.Out, Console.Error) { }

	public KaleidoscopeLexer(ICharStream input, TextWriter output, TextWriter errorOutput)
	: base(input, output, errorOutput)
	{
		Interpreter = new LexerATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	private static readonly string[] _LiteralNames = {
		null, "';'", "'{'", "'}'", "'('", "')'", "','", null, "'def'", "'extern'"
	};
	private static readonly string[] _SymbolicNames = {
		null, null, null, null, null, null, null, "WHITESPACE", "DEF", "EXTERN", 
		"NUMBER", "IDENTIFIER", "OP"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Kaleidoscope.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override string[] ChannelNames { get { return channelNames; } }

	public override string[] ModeNames { get { return modeNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static KaleidoscopeLexer() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}
	private static int[] _serializedATN = {
		4,0,12,89,6,-1,2,0,7,0,2,1,7,1,2,2,7,2,2,3,7,3,2,4,7,4,2,5,7,5,2,6,7,6,
		2,7,7,7,2,8,7,8,2,9,7,9,2,10,7,10,2,11,7,11,2,12,7,12,2,13,7,13,2,14,7,
		14,2,15,7,15,1,0,1,0,1,1,1,1,1,2,1,2,1,3,1,3,1,4,1,4,1,5,1,5,1,6,4,6,47,
		8,6,11,6,12,6,48,1,6,1,6,1,7,1,7,1,7,1,7,1,8,1,8,1,8,1,8,1,8,1,8,1,8,1,
		9,4,9,65,8,9,11,9,12,9,66,1,10,1,10,5,10,71,8,10,10,10,12,10,74,9,10,1,
		11,1,11,1,12,1,12,1,13,1,13,1,14,1,14,1,15,1,15,1,15,1,15,3,15,88,8,15,
		0,0,16,1,1,3,2,5,3,7,4,9,5,11,6,13,7,15,8,17,9,19,10,21,11,23,0,25,0,27,
		0,29,0,31,12,1,0,4,3,0,9,10,12,13,32,32,2,0,46,46,48,57,2,0,65,90,97,122,
		3,0,48,57,65,90,97,122,90,0,1,1,0,0,0,0,3,1,0,0,0,0,5,1,0,0,0,0,7,1,0,
		0,0,0,9,1,0,0,0,0,11,1,0,0,0,0,13,1,0,0,0,0,15,1,0,0,0,0,17,1,0,0,0,0,
		19,1,0,0,0,0,21,1,0,0,0,0,31,1,0,0,0,1,33,1,0,0,0,3,35,1,0,0,0,5,37,1,
		0,0,0,7,39,1,0,0,0,9,41,1,0,0,0,11,43,1,0,0,0,13,46,1,0,0,0,15,52,1,0,
		0,0,17,56,1,0,0,0,19,64,1,0,0,0,21,68,1,0,0,0,23,75,1,0,0,0,25,77,1,0,
		0,0,27,79,1,0,0,0,29,81,1,0,0,0,31,87,1,0,0,0,33,34,5,59,0,0,34,2,1,0,
		0,0,35,36,5,123,0,0,36,4,1,0,0,0,37,38,5,125,0,0,38,6,1,0,0,0,39,40,5,
		40,0,0,40,8,1,0,0,0,41,42,5,41,0,0,42,10,1,0,0,0,43,44,5,44,0,0,44,12,
		1,0,0,0,45,47,7,0,0,0,46,45,1,0,0,0,47,48,1,0,0,0,48,46,1,0,0,0,48,49,
		1,0,0,0,49,50,1,0,0,0,50,51,6,6,0,0,51,14,1,0,0,0,52,53,5,100,0,0,53,54,
		5,101,0,0,54,55,5,102,0,0,55,16,1,0,0,0,56,57,5,101,0,0,57,58,5,120,0,
		0,58,59,5,116,0,0,59,60,5,101,0,0,60,61,5,114,0,0,61,62,5,110,0,0,62,18,
		1,0,0,0,63,65,7,1,0,0,64,63,1,0,0,0,65,66,1,0,0,0,66,64,1,0,0,0,66,67,
		1,0,0,0,67,20,1,0,0,0,68,72,7,2,0,0,69,71,7,3,0,0,70,69,1,0,0,0,71,74,
		1,0,0,0,72,70,1,0,0,0,72,73,1,0,0,0,73,22,1,0,0,0,74,72,1,0,0,0,75,76,
		5,43,0,0,76,24,1,0,0,0,77,78,5,45,0,0,78,26,1,0,0,0,79,80,5,42,0,0,80,
		28,1,0,0,0,81,82,5,60,0,0,82,30,1,0,0,0,83,88,3,23,11,0,84,88,3,25,12,
		0,85,88,3,27,13,0,86,88,3,29,14,0,87,83,1,0,0,0,87,84,1,0,0,0,87,85,1,
		0,0,0,87,86,1,0,0,0,88,32,1,0,0,0,5,0,48,66,72,87,1,6,0,0
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
