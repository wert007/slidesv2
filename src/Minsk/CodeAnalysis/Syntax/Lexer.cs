using Minsk.CodeAnalysis.Symbols;
using Minsk.CodeAnalysis.Text;
using System.Globalization;

namespace Minsk.CodeAnalysis.Syntax
{
	internal sealed class Lexer
	{
		private readonly DiagnosticBag _diagnostics;
		private readonly SourceText _text;

		private bool _isAdvancedString;
		private bool _mustReadStringNext;


		private int _position;


		private int _start;
		private SyntaxKind _kind;
		private object _value;

		public Lexer(SourceText text)
		{
			_text = text;
			_mustReadStringNext = false;
			_isAdvancedString = false;
			_diagnostics = new DiagnosticBag(text.FileName);
		}

		public DiagnosticBag Diagnostics => _diagnostics;

		private char Current => Peek(0);

		private char Lookahead => Peek(1);

		private char Peek(int offset)
		{
			var index = _position + offset;

			if (index >= _text.Length)
				return '\0';

			return _text[index];
		}

		public SyntaxToken Lex()
		{
			_start = _position;
			_kind = SyntaxKind.BadToken;
			_value = null;

			if (_mustReadStringNext)
			{
				_mustReadStringNext = false;
				ReadString(0);
			}
			else
				SetToken();

			var length = _position - _start;
			var text = SyntaxFacts.GetText(_kind);
			if (text == null)
				text = _text.ToString(_start, length);

			return new SyntaxToken(_kind, _start, text, _value);
		}

		private void SetToken()
		{
			switch (Current)
			{
				case '\0':
					_kind = SyntaxKind.EndOfFileToken;
					break;
				case '+':
					_position++;
					if (Current == '=')
					{
						_kind = SyntaxKind.PlusEqualsToken;
						_position++;
					}
					else
					{
						_kind = SyntaxKind.PlusToken;
					}
					break;
				case '-':
					_position++;
					if (Current == '=')
					{
						_position++;
						_kind = SyntaxKind.MinusEqualsToken;
					}
					else
					{
						_kind = SyntaxKind.MinusToken;
					}
					break;
				case '*':
					_position++;
					if (Current == '=')
					{
						_position++;
						_kind = SyntaxKind.StarEqualsToken;
					}
					else
					{
						_kind = SyntaxKind.StarToken;
					}
					break;
				case '/':
					_position++;
					if (Current == '/')
					{
						while(Current != '\n')
						{
							_position++;
						}
						_kind = SyntaxKind.CommentToken;
					}
					else if(Current == '=')
					{
						_position++;
						_kind = SyntaxKind.SlashEqualsToken;
					}
					else
					{
						_kind = SyntaxKind.SlashToken;
					}
					break;
				case '%':
					_kind = SyntaxKind.PercentToken;
					_position++;
					break;
				case '(':
					_kind = SyntaxKind.OpenParenthesisToken;
					_position++;
					break;
				case ')':
					_kind = SyntaxKind.CloseParenthesisToken;
					_position++;
					break;
				case '~':
					_kind = SyntaxKind.TildeToken;
					_position++;
					break;
				case '^':
					_kind = SyntaxKind.HatToken;
					_position++;
					break;
				case ':':
					_kind = SyntaxKind.ColonToken;
					_position++;
					break;
				case ';':
					_kind = SyntaxKind.SemicolonToken;
					_position++;
					break;
				case ',':
					_kind = SyntaxKind.CommaToken;
					_position++;
					break;
				case '.':
					_kind = SyntaxKind.PeriodToken;
					_position++;
					if(Current == '.')
					{
						_kind = SyntaxKind.PeriodPeriodToken;
						_position++;
					}
					break;
				case '?':
					_kind = SyntaxKind.QuestionMarkToken;
					_position++;
					break;
				case '[':
					_kind = SyntaxKind.OpenBracketToken;
					_position++;
					break;
				case ']':
					_kind = SyntaxKind.CloseBracketToken;
					_position++;
					break;

				case '&':
					_position++;
					if (Current != '&')
					{
						_kind = SyntaxKind.BadToken;
					}
					else
					{
						_kind = SyntaxKind.AmpersandAmpersandToken;
						_position++;
					}
					break;
				case '|':
					_position++;
					if (Current != '|')
					{
						_kind = SyntaxKind.PipeToken;
					}
					else
					{
						_kind = SyntaxKind.PipePipeToken;
						_position++;
					}
					break;
				case '=':
					_position++;
					if(Current == '>')
					{
						_kind = SyntaxKind.EqualsGreaterToken;
						_position++;
					}
					else if(Current == '=')
					{
						_kind = SyntaxKind.EqualsEqualsToken;
						_position++;
					}
					else
					{
						_kind = SyntaxKind.EqualsToken;
					}
					break;
				case '!':
					_position++;
					if (Current != '=')
					{
						_kind = SyntaxKind.BangToken;
					}
					else
					{
						_kind = SyntaxKind.BangEqualsToken;
						_position++;
					}
					break;
				case '<':
					_position++;
					if (Current != '=')
					{
						_kind = SyntaxKind.LessToken;
					}
					else
					{
						_kind = SyntaxKind.LessOrEqualsToken;
						_position++;
					}
					break;
				case '>':
					_position++;
					if (Current != '=')
					{
						_kind = SyntaxKind.GreaterToken;
					}
					else
					{
						_kind = SyntaxKind.GreaterOrEqualsToken;
						_position++;
					}
					break;
				case '$':
					_kind = SyntaxKind.DollarSignToken;
					_position++;
					_isAdvancedString = true;
					break;
				case '@':
					ReadString(2);
					break;
				case '\'':
					ReadString();
					break;
				case '{':
					_position++;
					if (!_isAdvancedString)
						_kind = SyntaxKind.BadToken;
					else
						_kind = SyntaxKind.OpenBraceToken;
					break;
				case '}':
					_position++;
					if (!_isAdvancedString)
						_kind = SyntaxKind.BadToken;
					else
					{
						_kind = SyntaxKind.CloseBraceToken;
						_mustReadStringNext = true;
					}
					break;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
					ReadNumber();
					break;
				case ' ':
				case '\t':
				case '\n':
				case '\r':
					ReadWhiteSpace();
					break;
				default:
					if (char.IsLetter(Current))
					{
						ReadIdentifierOrKeyword();
					}
					else if (char.IsWhiteSpace(Current))
					{
						ReadWhiteSpace();
					}
					else
					{
						_diagnostics.ReportBadCharacter(_position, Current);
						_position++;
					}
					break;
			}
		}

		private void ReadWhiteSpace()
		{
			while (char.IsWhiteSpace(Current))
				_position++;

			_kind = SyntaxKind.WhitespaceToken;
		}

		private void ReadNumber()
		{
			while (char.IsDigit(Current))
				_position++;

			var isFloat = false;
			var hasSuffix = false;
			if(Current == '.' && Peek(1) != '.')
			{
				isFloat = true;
				_position++;
			}

			while (char.IsDigit(Current))
				_position++;

			if (Current == 'f')
			{
				isFloat = true;
				hasSuffix = true;
				_position++;
			}
			var length = _position - _start;
			var numLen = hasSuffix ? length - 1 : length;
			if (isFloat)
			{

				var text = _text.ToString(_start, numLen);
				if (!float.TryParse(text, NumberStyles.Float, CultureInfo.CreateSpecificCulture("US-us"), out var value))
					_diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, PrimitiveTypeSymbol.Float);
				_value = value;
			}
			else
			{
				var text = _text.ToString(_start, length);
				if (!int.TryParse(text, out var value))
					_diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, PrimitiveTypeSymbol.Integer);
				_value = value;
			}
			_kind = SyntaxKind.NumberToken;
		}



		private void ReadString(int trimStart = 1)
		{
			bool trimEnd = false;
			bool replaceBackSlash = false;

			if (Current == '@')
				replaceBackSlash = true;

			_position += trimStart;
			while (Current != '\'' && Current != '\0' && Current != '{')
				_position++;
			if (Current != '\'' && Current != '{')
				_diagnostics.ReportMissingCharacter(_start, '\'', '\'');
			else
				trimEnd = Current == '\'';
			if (trimEnd)
				_position++;

			var length = _position - _start;
			var start = _start + trimStart;
			var strLength = length - trimStart - (trimEnd ? 1 : 0);
			if (length > 1)
				_value = _text.ToString(start, strLength);
			else
				_value = string.Empty;

			if (replaceBackSlash)
				_value = _value.ToString().Replace("\\", "\\\\");

			_kind = SyntaxKind.StringToken;
		}

		private void ReadIdentifierOrKeyword()
		{
			while (char.IsLetter(Current))
				_position++;

			var length = _position - _start;
			var text = _text.ToString(_start, length);
			_kind = SyntaxFacts.GetKeywordKind(text);
		}
	}
}