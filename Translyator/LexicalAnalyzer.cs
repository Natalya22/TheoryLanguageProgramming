using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Translyator
{
	struct NameTok
	{
		public string name;
		public token tok;
	}
	class LexicalAnalyzer
  {
		public Dictionary<string, token> _userIdent = new Dictionary<string, token>();
		public Dictionary<string, token> _userNumber = new Dictionary<string, token>();
		public List<NameTok> _tokens = new List<NameTok>();
		public static int _sizeTokens = 0;
		public int _numUserIdent = 0;
		public int _numUserNumber = 0;
		public int _curToken = 0;

		public string WorkingLecsAnalyze(string userCode)
		{
			addStandartTypes2TableIdent();
			int textPtr = 0, sost;
			char placeholder;
			while (textPtr < userCode.Length)
			{
				StringBuilder unit = new StringBuilder();
				sost = 0;
				int stringPtr = 0;
				do
				{
					if (textPtr == userCode.Length)
					{
						sost = -21;
						textPtr++;
						unit.Append(' ');
						break;
					}
					unit.Append(userCode[textPtr]);
					placeholder = userCode[textPtr];
					if (
								 ((userCode[textPtr] >= 'a') && (userCode[textPtr] <= 'z'))
							|| ((userCode[textPtr] >= 'A') && (userCode[textPtr] <= 'Z'))
							|| (userCode[textPtr] == '_')
						)
					{
						if (!((sost == 6) && (userCode[textPtr] == 'e')))
						{
							placeholder = 'l';
						}
					}
					else if ((userCode[textPtr] >= '0') && (userCode[textPtr] <= '9'))
					{
						placeholder = 'd';
					}
					int[] machState = new int[14];
					if (!CommonData.Table.TryGetValue(placeholder.ToString(), out machState))
					{
						machState = CommonData.Table["o"];
					}
					sost = machState[sost];
					textPtr++;
					stringPtr++;
				} while (sost > 0);
				if (sost > -27)
				{
					if (    !((sost <= -9) && (sost >= -20))
								&&!((sost <= -1) && (sost >= -4))  
								&& (sost != -6) 
								&& (sost != -8)
								&& (sost != -26)
							)
					{
						textPtr--;
						stringPtr--;
						unit.Remove(unit.Length-1, 1);
					}

					token temp_tok = new token();
					temp_tok.attrib = 0;
					if (sost >= -24)
					{
						string name = "";
						foreach (var endSost in CommonData.SostAndToken.Keys)
						{
							if (sost == endSost)
							{
								ViewToken val = new ViewToken();
								CommonData.SostAndToken.TryGetValue(endSost, out val);
								temp_tok = val.tok;
								name = temp_tok.name;
							}
						}
						string normUnit = unit.ToString();
						if (name == "id")
						{
							int index, l = 0, h = 14;
							bool findKeyWord = false;
							var names = CommonData.KeyWords.Keys.ToArray();
							var tokens = CommonData.KeyWords.Values.ToArray();
							while (l <= h)
							{
								index = (l + h) / 2;
								if (normUnit.Equals(names[index]))
								{
									name = tokens[index].name;
									findKeyWord = true;
									l = h + 1;
								}
								else if (String.Compare(normUnit, names[index]) < 0)
								{
									h = index - 1;
								}
								else if (String.Compare(normUnit, names[index]) > 0)
								{
									l = index + 1;
								}
							}
							if (!findKeyWord)
							{
								if (!_userIdent.TryGetValue(normUnit, out temp_tok))
								{
									temp_tok.code = 28;
									temp_tok.attrib = _numUserIdent + 1;
									_userIdent.Add(normUnit, temp_tok);
									_numUserIdent++;
								}
							}
						}
						else if (name == "num")
						{
							if (!_userNumber.TryGetValue(normUnit, out temp_tok))
							{
								temp_tok.code = 29;
								temp_tok.attrib = _numUserNumber + 1;
								if (normUnit.Contains(".") || normUnit.Contains("e"))
								{
									temp_tok.tip = "float";
								}
								else
								{
									temp_tok.tip = "integer";
								}
								_userNumber.Add(normUnit, temp_tok);
								_numUserNumber++;
							}
						}
						Console.WriteLine("<" + name + "," + temp_tok.attrib + ">\n");
						NameTok temp = new NameTok();
						temp.name = name;
						temp.tok = temp_tok;
						if (name == "fls" || name == "tru")
						{
							temp.tok.tip = "boolean";
						}
						_tokens.Add(temp);
					}
				}
				else if (sost == -27)
				{
					Console.WriteLine("лексема не может начинаться с данного символа (номер состояния –27);");
					return "Лексема не может начинаться с данного символа (номер состояния –27);";
				}
				else if (sost == -28)
				{
					Console.WriteLine("недопустимый символ в обозначении числовой константы(номер состояния –28)");
					return "Недопустимый символ в обозначении числовой константы(номер состояния –28)";
				}
			}
			return "ok";
		}
		public void addStandartTypes2TableIdent()
		{
			token token = new token();
			token.code = 28;
			token.name = "id";
			token.cat = (int)cat.type_name;
			token.tip = null;
			token.attrib = _numUserIdent + 1;
			_userIdent.Add("boolean", token);
			_numUserIdent++;

			token.attrib = _numUserIdent + 1;
			_userIdent.Add("float", token);
			_numUserIdent++;

			token.attrib = _numUserIdent + 1;
			_userIdent.Add("integer", token);
			_numUserIdent++;
		}
		public void nextToken(out NameTok nameTokTip)
		{
			_sizeTokens = _tokens.Count;
			if (_curToken == _sizeTokens)
			{
				nameTokTip.name = "";
				nameTokTip.tok = new token();
				return;
			}
			nameTokTip.name = _tokens[_curToken].name;
			nameTokTip.tok = _tokens[_curToken].tok;
			_curToken++;
		}

	}
}
