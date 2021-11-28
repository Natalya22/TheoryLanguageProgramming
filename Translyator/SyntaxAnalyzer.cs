using System;
using System.Xml;
using System.Collections.Generic;

namespace Translyator
{
  class SyntaxAnalyzer
  {
    public Stack<token> _attrSt = new Stack<token>();
    public Stack<string> _typeSt = new Stack<string>();
    public Dictionary<string, List<NameTok>> _nameRecordAndFields = new Dictionary<string, List<NameTok>>();
    public List<NameTok> _recordFields = new List<NameTok>();
    public Stack<string> _codeSt = new Stack<string>();
    public List<string> _codeLines = new List<string>();
    public int _codeLineNum = 0;
    public int _codeTmpIndex = 0;
    public Stack<List<int>> _codeBoolSt = new Stack<List<int>>();
    public Stack<int> _parserState = new Stack<int>();
    struct _elementTR
    {
      public int ElType;
      public int ElPar;
      public string Left;
      public string Act;
    }
    public string analyze(LexicalAnalyzer lexicalAnalyzer)
    {
      _parserState.Push(1);
      NameTok currentToken = new NameTok();
      lexicalAnalyzer.nextToken(out currentToken);
      if (currentToken.name != "")
      {
        string lex = currentToken.name;
        int state = 0;
        NameTok predToken = new NameTok();
        _elementTR elementTR = new _elementTR();
        elementTR.ElType = (int)elType.elError;
        while (elementTR.ElType != (int)elType.elStop)
        {
          // ищем узел по состоянию и лексеме
          state = _parserState.Peek();
          XmlNode xmlNode = _getXmlNode(state, lex);
          if (xmlNode == null)
          {
            return "Xml узел не найден, состояние или лексема заданы неверно!";
          }

          // заполняем нашу структуру данными xml-узла
          _fillElTr(out elementTR, xmlNode);

          // в зависимости от типа д-ия выполняем следующий шаг
          switch (elementTR.ElType)
          {
            case (int)elType.elError: return "syntax error!";
            case (int)elType.elMove:
              if ((predToken.name == "lnch") && (elementTR.ElPar == 3))
              {
                if (!AddProgName(ref currentToken, ref lexicalAnalyzer))
                  return "Lexical error!";
              }
              _parserState.Push(elementTR.ElPar);
              if (CommonData.Terms.ContainsKey(lex))
              {
                if (currentToken.name == ".")
                {
                  _attrSt.Push(predToken.tok);
                }
                predToken = currentToken;
                lexicalAnalyzer.nextToken(out currentToken);
                if (currentToken.name == "")
                {
                  return "Lexical error!";
                }
              }
              lex = currentToken.name;
              break;
            case (int)elType.elTurn:
              for (int i = 1; i <= elementTR.ElPar; i++)
              {
                _parserState.Pop();
              }
              lex = elementTR.Left;
              if (elementTR.Act != "")
              {
                string result = _semanticAction(elementTR.Act, ref predToken, ref lexicalAnalyzer);
                if (result != "")
                {
                  return "Syntax error: " + result;
                }
              }
              break;
          }
        }
      }
      else
      {
        return "Program is empty!";
      }
      return "ok";
    }

    private string _semanticAction(string action, ref NameTok token, ref LexicalAnalyzer lexicalAnalyzer)
    {
      string lex = "";
      string type = "";
      token tok = new token();
      string type1 = "", type2 = "";
      string str1 = "", str2 = "";
      string varname = "";
      switch (action)
      {
        // Проверка, что это был тип или имя записи
        case "A1":
          if (!((token.tok.cat == (int)cat.type_name) || (token.tok.cat == (int)cat.record_name)))
          {
            return "лексема не является типом!";
          }
          _typeSt.Push(findLexInTable(token.tok, lexicalAnalyzer));
          break;
        // Проверка на повторение идентификатора
        case "A2":
          if (token.tok.cat != (int)cat.none)
          {
            return "идентификатор уже определен!";
          }
          _attrSt.Push(token.tok);
          break;
        // Присвоение типа переменной
        case "A3":
          type = _typeSt.Pop();
          while (_attrSt.Count > 0)
          {
            tok = _attrSt.Pop();
            lex = findLexInTable(tok, lexicalAnalyzer);
            tok.tip = type;
            tok.cat = (int)cat.identif_name;
            lexicalAnalyzer._userIdent.Remove(lex);
            lexicalAnalyzer._userIdent.Add(lex, tok);
          }
          break;
        // Установка, что это имя записи
        case "A4":
          if (token.tok.cat != 0)
          {
            return "такой идентификатор уже использовался!";
          }
          lex = findLexInTable(token.tok, lexicalAnalyzer);
          token.tok.cat = (int)cat.record_name;
          lexicalAnalyzer._userIdent.Remove(lex);
          lexicalAnalyzer._userIdent.Add(lex, token.tok);
          _typeSt.Push(lex);
          break;
        // Заполнение таблицы полей записи, относящейся к ней.
        case "A5":
          _nameRecordAndFields.Add(_typeSt.Peek(), _recordFields);
          break;
        // Установка типа Запись у переменных
        case "A6":
          string recName = _typeSt.Pop();
          for (int i = 0; i < _attrSt.Count; i++)
          {
            tok = _attrSt.Pop();
            lex = findLexInTable(tok, lexicalAnalyzer);
            tok.tip = recName;
            tok.cat = (int)cat.identif_name;
            lexicalAnalyzer._userIdent.Remove(lex);
            lexicalAnalyzer._userIdent.Add(lex, tok);
          }
          break;
        // Проверка, что полю записи устанавливается какой-либо тип, кроме записи
        case "A7":
          if (!(token.tok.cat == (int)cat.type_name))
          {
            return "лексема не является стандартным типом!";
          }
          _typeSt.Push(findLexInTable(token.tok, lexicalAnalyzer));
          break;
        // Добавление в список полей записи
        case "A8":
          NameTok nameTok = new NameTok();
          type = _typeSt.Pop();
          tok = _attrSt.Pop();
          tok.cat = (int)cat.identif_name;
          tok.tip = type;
          nameTok.tok = tok;
          nameTok.name = findLexInTable(tok, lexicalAnalyzer);
          // !!!
          lexicalAnalyzer._userIdent.Remove(nameTok.name);
          _recordFields.Add(nameTok);
          break;
        // BackPatch, nextlist
        case "A9":
          break;
        // На всякий случай оставила для LstStmt = LstStmt Stmt “;”
        case "A10":
          break;
        // Проверка, что идентификатор явл-ся именем переменной
        case "A11":
          lex = findLexInTable(token.tok, lexicalAnalyzer);
          if (!(lexicalAnalyzer._userIdent.TryGetValue(lex, out tok)))
          {
            return "такой переменной нет!";
          }
          if (!(tok.cat == (int)cat.identif_name))
          {
            return "неверный идентификатор!";
          }
          _typeSt.Push(tok.tip);
          _codeSt.Push('@' + lex);
          _codeBoolSt.Push(new List<int>());
          _codeBoolSt.Push(new List<int>());
          break;
        // Проверка, что типы совпадают
        case "A12":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 != type2)
          {
            return "типы переменных не совпадают";
          }
          else
          {
            string right = _codeSt.Pop();
            string left = _codeSt.Pop();
            Gen(left + '=' + right);
          }
          //List<int> B_false = _codeBoolSt.Pop();
          //List<int> B_true = _codeBoolSt.Pop();
          //BackPatch(B_true, _codeLineNum);
          //BackPatch(B_false, _codeLineNum + 2);
          //string str = _codeSt.Pop();
          //Gen(str + " = true");
          //Gen("goto " + (_codeLineNum + 2));
          //Gen(str + " = false");
          //_codeBoolSt.Clear();
          //_codeBoolSt = new Stack<List<int>>();
          break;
        // Проверка, что переменная есть имя записи
        case "A13":
          tok = _attrSt.Peek();
          lex = findLexInTable(tok, lexicalAnalyzer);
          if (!(lexicalAnalyzer._userIdent.TryGetValue(lex, out tok)))
          {
            return "такого идентификатора нет!";
          }
          string nameType = tok.tip;
          if (!(_nameRecordAndFields.ContainsKey(nameType)))
          {
            return "такой записи нет!";
          }
          break;
        // Проверка, что переменная относится к этой записи.
        case "A14":
          tok = _attrSt.Pop();
          lex = findLexInTable(tok, lexicalAnalyzer);
          string rName = lex;
          lexicalAnalyzer._userIdent.TryGetValue(lex, out tok);
          List <NameTok> lstFields = new List<NameTok>();
          if (!_nameRecordAndFields.TryGetValue(tok.tip, out lstFields))
          {
            return "такой записи нет!";
          }
          bool found = false;
          string fName = "";
          for (int i = 0; i < lstFields.Count; i++)
          {
            if (lstFields[i].tok.attrib == token.tok.attrib)
            {
              found = true;
              tok = lstFields[i].tok;
              fName = lstFields[i].name;
            }
          }
          if (!found)
          {
            return "у записи нет такого поля!";
          }
          if (tok.cat != (int)cat.identif_name)
          {
            return "поле записи не является идентификатором!";
          }
          _typeSt.Push(tok.tip);
          _codeSt.Push('@'+rName+'.'+fName);
          _codeBoolSt.Push(new List<int>());
          _codeBoolSt.Push(new List<int>());
          break;
        // Проверка, что типы совпадают с типом поля записи
        case "A15":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 == "boolean")
          {
            if (token.name == "fls")
            {
              _codeSt.Push("false");
            }
            else
            {
              _codeSt.Push("true");
            }
          }
          if (type1 != type2)
          {
            return "типы переменных не совпадают";
          }
          else
          {
            string right = _codeSt.Pop();
            string left = _codeSt.Pop();
            Gen(left + '=' + right);
          }
          //List<int> B_false = _codeBoolSt.Pop();
          //List<int> B_true = _codeBoolSt.Pop();
          //BackPatch(B_true, _codeLineNum);
          //BackPatch(B_false, _codeLineNum + 2);
          //string str = _codeSt.Pop();
          //Gen(str + " = true");
          //Gen("goto " + (_codeLineNum + 2));
          //Gen(str + " = false");
          //_codeBoolSt.Clear();
          //_codeBoolSt = new Stack<List<int>>();
          break;
        // Проверка, что это булева переменная
        case "A16":
          break;
        // Условие if
        case "A17":
          break;
        // Expr = SmpExpr
        case "A18":
          break;
        // SmpExpr “rel” SmpExpr
        case "A19":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 == "boolean" || type2 == "boolean")
          {
            return "булевский тип не ожидался!";
          }

          _typeSt.Push("boolean");

          string typ = type2;
          if (type == "integer" || type == "float")
          {
            //CodeSt.push("@" + LexA._identif[token.attr]._lex);
          }
          else
          {
            return "Type error";
          }

          _codeBoolSt.Push(new List<int>(_codeLineNum));
          _codeBoolSt.Push(new List<int>(_codeLineNum));
          string tmp1 = _codeSt.Pop();
          string tmp2 = _codeSt.Pop();
          Gen("if " + tmp2 + " REL_OP " + tmp1 + " goto ?");
          Gen("goto ?");
          break;
        // SmpExpr = Term 
        case "A20":
          break;
        // SmpExpr = SmpExpr “+” Term
        case "A21":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 != type2)
          {
            return "Type error";
          }
          _typeSt.Push(type1);

          str1 = _codeSt.Pop();
          str2 = _codeSt.Pop();
          varname = "$tmp" + _codeTmpIndex.ToString();
          _codeTmpIndex++;
          Gen(varname + " = " + str1 + " + " + str2);
          _codeSt.Push(varname);
          break;
        // or
        case "A22":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 != "boolean" || type2 != "boolean")
          {
            return "Type error (boolean value expected)";
          }
          _typeSt.Push("boolean");
          //List<int> B2_false = _codeBoolSt.Pop();
          //List<int> B2_true = _codeBoolSt.Pop();
          //List<int> M_instr = _codeBoolSt.Pop();
          //List<int> B1_false = _codeBoolSt.Pop();
          //List<int> B1_true = _codeBoolSt.Pop();
          //backpatch(B1_false, M_instr)
          //B1_true.AddRange(B2_true);
          //_codeBoolSt.Push(B1_true);
          //_codeBoolSt.Push(B2_false);
          break;
        case "A23":
          break;
        // *
        case "A24":
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 != type2)
          {
            return "Type error";
          }
          _typeSt.Push(type1);

          str1 = _codeSt.Pop();
          str2 = _codeSt.Pop();
          varname = "$tmp" + _codeTmpIndex.ToString();
          _codeTmpIndex++;
          Gen(varname + " = " + str1 + " * " + str2);
          _codeSt.Push(varname);
          break;
        case "A25":
        // and
          type1 = _typeSt.Pop();
          type2 = _typeSt.Pop();
          if (type1 != "boolean" || type2 != "boolean")
          {
            return "Type error (boolean value expected)";
          }
          _typeSt.Push("boolean");
          //List<int> B2_false = _codeBoolSt.Pop();
          //List<int> B2_true = _codeBoolSt.Pop();
          //List<int> M_instr = _codeBoolSt.Pop();
          //List<int> B1_false = _codeBoolSt.Pop();
          //List<int> B1_true = _codeBoolSt.Pop();
          //_codeBoolSt.Push(B2_true);
          //B1_false.AddRange(B2_false);
          //_codeBoolSt.Push(B1_false.concat(B2_false));
          break;
        case "A26":
          break;
        case "A27":
          // !!! Maybe need add type to stack
          break;
        case "A30":
          if (_typeSt.Peek() != "boolean")
          {
            return "ожидался булевский тип!";
          }
          break;
        case "A31":
          lex = findNumInTable(token.tok, lexicalAnalyzer);
          _typeSt.Push(token.tok.tip);
          _codeSt.Push("#" + lex);
          break;
        // true
        case "A32":
          _typeSt.Push("boolean");
          _codeBoolSt.Push(new List<int>(_codeLineNum));
          _codeBoolSt.Push(new List<int>());
          //Gen("goto ?");
          break;
        // false
        case "A33":
          _typeSt.Push("boolean");
          _codeBoolSt.Push(new List<int>());
          _codeBoolSt.Push(new List<int>(_codeLineNum));
          //Gen("goto ?");
          break;
        case "A34":
          break;
      }
      return "";
    }

    private XmlNode _getXmlNode(int state, string lex)
    {
      XmlNodeList xmlNodes = CommonData.xml.SelectNodes("SLR_Table/Row");
      foreach (XmlNode node in xmlNodes)
      {
        if (state == Int32.Parse(node.Attributes.GetNamedItem("NSost").Value))
        {
          XmlNodeList subXmlNodes = node.ChildNodes;
          int colNum = -1;
          if (CommonData.NeterminalsAndTerms.TryGetValue(lex, out colNum))
          {
            foreach (XmlNode subNode in subXmlNodes)
            {
              if (colNum == Int32.Parse(subNode.Attributes.GetNamedItem("ColNum").Value))
              {
                return subNode;
              }
            }
          }
          else
          {
            return null;
          }
        }
      }
      return null;
    }

    private void _fillElTr(out _elementTR elementTR, XmlNode xmlNode)
    {
      elementTR.ElType = Int32.Parse(xmlNode.Attributes.GetNamedItem("ElType").Value);
      elementTR.ElPar = Int32.Parse(xmlNode.Attributes.GetNamedItem("ElPar").Value);
      elementTR.Left = xmlNode.Attributes.GetNamedItem("Left").Value;
      elementTR.Act = xmlNode.Attributes.GetNamedItem("Act").Value;
    }

    private bool AddProgName(ref NameTok NameTok, ref LexicalAnalyzer lexicalAnalyzer)
    {
      string key = findLexInTable(NameTok.tok, lexicalAnalyzer);
      if (key != "")
      {
        NameTok.tok.cat = (int)cat.program_name;
        NameTok.tok.tip = null;
        lexicalAnalyzer._userIdent.Remove(key);
        lexicalAnalyzer._userIdent.Add(key, NameTok.tok);
        return true;
      }
      return false;
    }

    private string findLexInTable(token token, LexicalAnalyzer lexicalAnalyzer)
    {
      string key = "";
      int index = 0;
      foreach (var val in lexicalAnalyzer._userIdent.Values)
      {
        if (val.attrib == token.attrib)
        {
          break;
        }
        else
        {
          index++;
        }
      }
      string[] keys = new string[lexicalAnalyzer._userIdent.Keys.Count];
      lexicalAnalyzer._userIdent.Keys.CopyTo(keys, 0);
      key = keys[index];
      return key;
    }

    private string findNumInTable(token token, LexicalAnalyzer lexicalAnalyzer)
    {
      string key = "";
      int index = 0;
      foreach (var val in lexicalAnalyzer._userNumber.Values)
      {
        if (val.attrib == token.attrib)
        {
          break;
        }
        else
        {
          index++;
        }
      }
      string[] keys = new string[lexicalAnalyzer._userIdent.Keys.Count];
      lexicalAnalyzer._userNumber.Keys.CopyTo(keys, 0);
      key = keys[index];
      return key;
    }

    private void Gen(string str)
    {
      _codeLines.Add(_codeLineNum.ToString() + " : " + str);
      _codeLineNum++;
    }
    void BackPatch(List<int> list, int codeline)
    {
      string str = "";
      foreach (int val in list)
      {
        str = _codeLines.ToArray()[val];
        str += " *" + codeline.ToString();
        _codeLines[val] = str;
      }
    }
  }
}
