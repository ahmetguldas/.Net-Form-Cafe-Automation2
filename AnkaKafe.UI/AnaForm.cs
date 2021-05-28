using AnkaKafe.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnkaKafe.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db;
        public AnaForm()
        {
            VerileriOku();
            InitializeComponent();
            Icon = Resource.icon;
            masalarImageList.Images.Add("bos", Resource.bos);
            masalarImageList.Images.Add("dolu", Resource.dolu);
            MasalariOlustur();
        }

        private void VerileriOku()
        {
            try
            {
                string json = File.ReadAllText("veri.json");
                db = JsonSerializer.Deserialize<KafeVeri>(json);
            }
            catch (Exception)
            {

                db = new KafeVeri();
                OrnekUrunleriEkle();
            }
        }

        private void OrnekUrunleriEkle()
        {
            db.Urunler.Add(new Urun() { UrunAd = "Cay", BirimFiyat = 4.00m });
            db.Urunler.Add(new Urun() { UrunAd = "Simit", BirimFiyat = 5.00m });
        }

        private void MasalariOlustur()
        {
            ListViewItem lvi;
            for (int i = 1; i <= db.MasaAdet; i++)
            {
                lvi = new ListViewItem();
                lvi.Tag = i; //Masa noyu her bir ogenin Tag property"sinde saklayalim.
                lvi.Text = "Masa " + i;
                lvi.ImageKey = MasaDoluMu(i) ? "dolu" : "bos";
                lvwMasalar.Items.Add(lvi);
            }
        }

        private bool MasaDoluMu(int masaNo)
        {
            return db.AktifSiparisler.Any(x => x.MasaNo == masaNo);
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == tsmiUrunler)
            {
                new UrunlerForm(db).ShowDialog();
            }
            else if (e.ClickedItem == tsmiGecmisSiparisler)
            {
                new GecmisSiparislerForm(db).ShowDialog();
            }
        }

        private void lvwMasalar_DoubleClick(object sender, EventArgs e)
        {
            ListViewItem lvi = lvwMasalar.SelectedItems[0];
            int masaNo = (int)lvi.Tag; //unboxing yapmis olduk
            lvi.ImageKey = "dolu";

            //eger bu masada onceden bir siparis yoksa olustur.
            Siparis siparis = SiparisBul(masaNo);

            if (siparis == null)
            {
                siparis = new Siparis() { MasaNo = masaNo };
                db.AktifSiparisler.Add(siparis);
            }

            //todo: bu siparisi baska bir formda ac
            SiparisForm siparisForm = new SiparisForm(db, siparis);

            siparisForm.MasaTasindi += SiparisForm_MasaTasindi;

            siparisForm.ShowDialog();

            //Siparis formu kapandiktan sonra siparis durumunu kontrol et
            if (siparis.Durum != SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
            }
        }

        private void SiparisForm_MasaTasindi(object sender, MasaTasindiEventArgs e)
        {
            foreach (ListViewItem lvi in lvwMasalar.Items)
            {
                int masaNo = (int)lvi.Tag;
                if (masaNo == e.EskiMasaNo)
                {
                    lvi.ImageKey = "bos";
                }
                else if (masaNo == e.YeniMasaNo)
                {
                    lvi.ImageKey = "dolu";
                }

            }
        }

        private Siparis SiparisBul(int masaNo)
        {
            //return db.AktifSiparisler.FirstOrDefault(s => s.MasaNo == masaNo); //Asagidaki kodun kisa hali

            foreach (Siparis siparis in db.AktifSiparisler)
            {
                if (siparis.MasaNo == masaNo)
                {
                    return siparis;
                }
            }
            return null;
        }

        private void menuStrip1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void AnaForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            VerileriKaydet();
        }

        private void VerileriKaydet()
        {
            //https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-how-to?pivots=dotnet-5-0

            var options = new JsonSerializerOptions() { WriteIndented = true }; //json u okunakli indentation ile olusturur.
            string json = JsonSerializer.Serialize(db,options) ;
            File.WriteAllText("veri.json", json);
        }
    }
}
