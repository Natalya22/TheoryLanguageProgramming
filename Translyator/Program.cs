using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Translyator
{
  static class Program
  {
    /// <summary>
    /// Главная точка входа для приложения.
    /// </summary>
    [STAThread]
    static void Main()
    {
      // Общая инфа, связанная с лексическим анализатором
      CommonData.FillKeyWordsTable();
      CommonData.FillSostAndTokenTable();
      CommonData.FillTransitionTable();

      // Общая инфа, связанная с синтаксическим анализатором
      CommonData.loadXML(@"C:\Users\Lenovo\source\repos\Translyator\Translyator\NataLang\SLR_Table2.xml");
      CommonData.FillNeterminalsAndTerms();

      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      Application.Run(new Form1());
    }
  }
}
