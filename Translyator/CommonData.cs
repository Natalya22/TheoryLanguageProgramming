using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;

namespace Translyator
{
  public struct token
  {
    public int code;
    public string name;
    public int attrib;
    public int cat;
    public string tip;
  };

  public struct ViewToken
  {
    public token tok;
    public string view;
  };

  public enum cat
  {
    none         = 0,
    type_name    = 1,
    identif_name = 2,
    program_name = 3,
    record_name  = 4
  }

  public enum elType
  {
    elError = 0,
    elMove  = 1,
    elTurn  = 2,
    elStop  = 3
  }

  class CommonData
  {
    // Заполнение ключевых слов
    static public Dictionary<string, token> KeyWords = new Dictionary<string, token>();
    static public void FillKeyWordsTable()
    {
      try
      {
        using (StreamReader sr = new StreamReader(@"C:\Users\Lenovo\source\repos\Translyator\Translyator\NataLang\KeyWords.txt"))
        {
          string line;
          token tokenUnit = new token();
          string[] substrings = new string[4];
          while ((line = sr.ReadLine()) != null)
          {
            Console.WriteLine(line);
            substrings = line.Split(' ');
            Int32.TryParse(substrings[0], out tokenUnit.code);
            tokenUnit.name = substrings[1];
            Int32.TryParse(substrings[3], out tokenUnit.attrib);
            KeyWords.Add(substrings[2], tokenUnit);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    // Заполнение конечных состояний
    static public Dictionary<int, ViewToken> SostAndToken = new Dictionary<int, ViewToken>();
    static public void FillSostAndTokenTable()
    {
      try
      {
        using (StreamReader sr = new StreamReader(@"C:\Users\Lenovo\source\repos\Translyator\Translyator\NataLang\Leksems.txt"))
        {
          string line;
          ViewToken viewTokenUnit = new ViewToken();
          string[] substrings = new string[5];
          while ((line = sr.ReadLine()) != null)
          {
            Console.WriteLine(line);
            substrings = line.Split(' ');
            Int32.TryParse(substrings[1], out viewTokenUnit.tok.code);
            viewTokenUnit.tok.name = substrings[2];
            viewTokenUnit.view = substrings[3];
            Int32.TryParse(substrings[4], out viewTokenUnit.tok.attrib);
            SostAndToken.Add(Int32.Parse(substrings[0]), viewTokenUnit);
          }
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.Message);
      }
    }

    // Заполнение таблицы состояний автомата
    static public Dictionary<string, int[]> Table = new Dictionary<string, int[]>();
    static public void FillTransitionTable()
    {
      int[] machineState = new int[14];
      machineState[0] = 1;
      machineState[1] = -1;
      machineState[2] = -3;
      machineState[3] = -5;
      machineState[4] = -7;
      machineState[5] = -21;
      machineState[6] = -22;
      machineState[7] = -28;
      machineState[8] = -23;
      machineState[9] = -28;
      machineState[10] = -28;
      machineState[11] = -24;
      machineState[12] = -25;
      machineState[13] = 13;
      Table["<"] = new int[14];
      for (int i = 0; i < 14; i++) Table["<"][i] = machineState[i];

      machineState[0] = 2;
      Table[">"] = new int[14];
      for (int i = 0; i < 14; i++) Table[">"][i] = machineState[i];

      machineState[0] = 4;
      Table["!"] = new int[14];
      for (int i = 0; i < 14; i++) Table["!"][i] = machineState[i];

      machineState[0] = -9;
      Table[";"] = new int[14];
      for (int i = 0; i < 14; i++) Table[";"][i] = machineState[i];

      machineState[0] = -10;
      machineState[6] = -28;
      Table[":"] = new int[14];
      for (int i = 0; i < 14; i++) Table[":"][i] = machineState[i];

      machineState[0] = -11;
      Table[","] = new int[14];
      for (int i = 0; i < 14; i++) Table[","][i] = machineState[i];

      machineState[0] = -12;
      machineState[6] = 7;
      Table["."] = new int[14];
      for (int i = 0; i < 14; i++) Table["."][i] = machineState[i];

      machineState[0] = -13;
      machineState[6] = -28;
      Table["("] = new int[14];
      for (int i = 0; i < 14; i++) Table["("][i] = machineState[i];

      machineState[0] = -14;
      machineState[6] = -22;
      Table[")"] = new int[14];
      for (int i = 0; i < 14; i++) Table[")"][i] = machineState[i];

      machineState[0] = -15;
      machineState[6] = -28;
      Table["{"] = new int[14];
      for (int i = 0; i < 14; i++) Table["{"][i] = machineState[i];

      machineState[0] = -16;
      Table["}"] = new int[14];
      for (int i = 0; i < 14; i++) Table["}"][i] = machineState[i];

      machineState[0] = -17;
      machineState[6] = -22;
      machineState[9] = 10;
      Table["+"] = new int[14];
      for (int i = 0; i < 14; i++) Table["+"][i] = machineState[i];

      machineState[0] = -18;
      Table["-"] = new int[14];
      for (int i = 0; i < 14; i++) Table["-"][i] = machineState[i];

      machineState[9] = -28;

      machineState[0] = -19;
      Table["*"] = new int[14];
      for (int i = 0; i < 14; i++) Table["*"][i] = machineState[i];

      machineState[0] = -20;
      Table["/"] = new int[14];
      for (int i = 0; i < 14; i++) Table["/"][i] = machineState[i];

      // check in code if l in [a..z] and [A..Z]
      machineState[0] = machineState[5] = 5;
      // !!
      machineState[6] = -28;
      Table["l"] = new int[14];
      for (int i = 0; i < 14; i++) Table["l"][i] = machineState[i];

      // check in code if l in [0..9]
      machineState[0] = machineState[6] = 6;
      machineState[7] = machineState[8] = 8;
      machineState[9] = machineState[10] = machineState[11] = 11;
      Table["d"] = new int[14];
      for (int i = 0; i < 14; i++) Table["d"][i] = machineState[i];

      machineState[0] = 3;
      machineState[1] = -2;
      machineState[2] = -4;
      machineState[3] = -6;
      machineState[4] = -8;
      machineState[5] = -21;
      machineState[6] = -22;
      machineState[7] = -28;
      machineState[8] = -23;
      machineState[9] = -28;
      machineState[10] = -28;
      machineState[11] = -24;
      Table["="] = new int[14];
      for (int i = 0; i < 14; i++) Table["="][i] = machineState[i];

      machineState[0] = -27;
      machineState[1] = -1;
      machineState[2] = -3;
      machineState[3] = -5;
      machineState[4] = -7;
      machineState[5] = -21;
      machineState[6] = machineState[8] = 9;
      Table["e"] = new int[14];
      for (int i = 0; i < 14; i++) Table["e"][i] = machineState[i];

      machineState[0] = 13;
      machineState[6] = -22;
      machineState[8] = -23;
      machineState[13] = -26;
      Table["#"] = new int[14];
      for (int i = 0; i < 14; i++) Table["#"][i] = machineState[i];

      machineState[13] = 13;

      machineState[0] = 12;
      machineState[12] = 12;
      Table[" "] = new int[14];
      for (int i = 0; i < 14; i++) Table[" "][i] = machineState[i];
      Table["	"] = new int[14];
      for (int i = 0; i < 14; i++) Table["	"][i] = machineState[i];
      Table["\n"] = new int[14];
      for (int i = 0; i < 14; i++) Table["\n"][i] = machineState[i];
      // if another symbol, check in code
      machineState[0] = -27;
      machineState[12] = -25;
      Table["o"] = new int[14];
      for (int i = 0; i < 14; i++) Table["o"][i] = machineState[i];
    }

    public static XmlDocument xml = new XmlDocument();
    static public void loadXML(string path)
    {
      xml.Load(path);
    }

    // Заполнение нетерминалов и терминалов
    static public Dictionary<string, int> Neterminals = new Dictionary<string, int>();
    static public Dictionary<string, int> Terms = new Dictionary<string, int>();
    static public Dictionary<string, int> NeterminalsAndTerms = new Dictionary<string, int>();
    static public void FillNeterminalsAndTerms()
    {
      XmlNodeList xmlNodes = xml.SelectNodes("SLR_Table/Columns/Neterms/Column");
      foreach (XmlNode node in xmlNodes)
      {
        int colNum = Int32.Parse(node.Attributes.GetNamedItem("ColNum").Value);
        string lexeme = node.Attributes.GetNamedItem("Lexeme").Value;
        Neterminals.Add(lexeme, colNum);
        NeterminalsAndTerms.Add(lexeme, colNum);
      }
      xmlNodes = xml.SelectNodes("SLR_Table/Columns/Terms/Column");
      foreach (XmlNode node in xmlNodes)
      {
        int colNum = Int32.Parse(node.Attributes.GetNamedItem("ColNum").Value);
        string lexeme = node.Attributes.GetNamedItem("Lexeme").Value;
        Terms.Add(lexeme, colNum);
        NeterminalsAndTerms.Add(lexeme, colNum);
      }
    }
  }
}
