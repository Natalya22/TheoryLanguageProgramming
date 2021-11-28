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
      if (result != "ok")
      {
        Error.Text = result;
        return;
      }
      Output.Text = result;
    }
  }
}
