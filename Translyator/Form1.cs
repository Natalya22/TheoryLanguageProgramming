using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translyator
{
  public partial class Form1 : Form
  {
    LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();
    SyntaxAnalyzer syntaxAnalyzer = new SyntaxAnalyzer();
    public Form1()
    {
      InitializeComponent();
    }

    private void Compile_Click(object sender, EventArgs e)
    {
      string result = lexicalAnalyzer.WorkingLecsAnalyze(UserCode.Text);
      if (result != "ok")
      {
        Error.Text = result;
        return;
      }
      result = syntaxAnalyzer.analyze(lexicalAnalyzer);
      Error.Text = result;
      if (result != "ok")
      {
        return;
      }
      string res = "";
      for (int i = 0; i < syntaxAnalyzer._codeLines.Count; i++)
      {
        res += syntaxAnalyzer._codeLines[i] + '\n';
      }
      Output.Text = res;
    }

    private void button1_Click(object sender, EventArgs e)
    {
      LexicalAnalyzer._sizeTokens = 0;
      lexicalAnalyzer._userIdent.Clear();
      lexicalAnalyzer._userNumber.Clear();
      lexicalAnalyzer._tokens.Clear();
      lexicalAnalyzer._curToken = 0;
      lexicalAnalyzer._numUserIdent = 0;
      lexicalAnalyzer._numUserNumber = 0;

      syntaxAnalyzer._codeLines.Clear();
      syntaxAnalyzer._attrSt.Clear();
      syntaxAnalyzer._codeBoolSt.Clear();
      syntaxAnalyzer._codeLineNum = 0;
      syntaxAnalyzer._codeSt.Clear();
      syntaxAnalyzer._codeTmpIndex = 0;
      syntaxAnalyzer._nameRecordAndFields.Clear();
      syntaxAnalyzer._parserState.Clear();
      syntaxAnalyzer._recordFields.Clear();
      syntaxAnalyzer._typeSt.Clear();

      UserCode.Text = "";
      Output.Text = "";
      Error.Text = "";
    }
  }
}
