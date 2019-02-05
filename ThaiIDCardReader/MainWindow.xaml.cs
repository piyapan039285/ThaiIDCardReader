using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ThaiNationalIDCard;

namespace ThaiIDCardReader
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    private ThaiIDCard IDCardReader;

    public MainWindow()
    {
      InitializeComponent();
    }

    private async void Read_ID_Card_Click(object sender, RoutedEventArgs e)
    {
      ProgressLabel.Content = "Reading. Please Wait ...";
      CardData.Document.Blocks.Clear();
      ReadCardBtn.IsEnabled = false;

      //wait for the task to finish and returns back its results, without blocking the main thread because of the async keyword's magic ability
      await Task.Run(() => ReadCard());

      ReadCardBtn.IsEnabled = true;
    }

    internal void ReadCard()
    {
      string status = "";
      string data = "";

      try
      {
        if (IDCardReader == null)
          IDCardReader = new ThaiIDCard();

        string[] readers = IDCardReader.GetReaders();
        if (readers == null || readers.Length == 0)
        {
          status = "Card reader not found. Please try plug out/in again.";
          return;
        }

        Personal personData = IDCardReader.readAll();
        if (personData == null)
        {
          status = "Cannot read card. Please try again.";
          return;
        }

        data = "เลบบัตรประชาชน: " + personData.Citizenid + "\n" +
          "วันเกิด: " + personData.Birthday.ToString("dd/MM/yyyy") + "\n" +
          "เพศ: " + personData.Sex + "\n" +
          "ชื่อไทย: " + personData.Th_Prefix + personData.Th_Firstname + " " + personData.Th_Lastname + "\n" +
          "Name: " + personData.En_Prefix + personData.En_Firstname + " " + personData.En_Lastname + "\n" +
          "วันออกบัตร: " + personData.Issue.ToString("dd/MM/yyyy") + "\n" +
          "ผู้ออกบัตร: " + personData.Issuer + "\n" +
          "วันหมดอายุ: " + personData.Expire.ToString("dd/MM/yyyy") + "\n" +
          "ที่อยู่: " + personData.Address + "\n" +
          "บ้านเลขที่: " + personData.addrHouseNo + "\n" +
          "หมู่ที่: " + personData.addrVillageNo + "\n" +
          "ซอย: " + personData.addrLane + "\n" +
          "ถนน: " + personData.addrRoad + "\n" +
          "ตำบล: " + personData.addrTambol + "\n" +
          "อำเถอ: " + personData.addrAmphur + "\n" +
          "จังหวัด: " + personData.addrProvince + "\n";
      }
      finally
      {
        // update UI in main thread.
        Dispatcher.Invoke(() =>
        {
          ProgressLabel.Content = status;
          CardData.Document.Blocks.Clear();
          CardData.Document.Blocks.Add(new Paragraph(new Run(data)));
        });
      }
    }
  }
}
