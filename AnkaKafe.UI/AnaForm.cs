﻿using AnkaKafe.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AnkaKafe.UI
{
    public partial class AnaForm : Form
    {
        KafeVeri db = new KafeVeri();
        public AnaForm()
        {
            OrnekUrunleriEkle(); //Ileride Kaldirilacak (Ornek Urun Ekleme Islemi)
            InitializeComponent();
            masalarImageList.Images.Add("bos", Resource.bos);
            masalarImageList.Images.Add("dolu", Resource.dolu);
            MasalariOlustur();
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
                lvi.ImageKey = "bos";
                lvwMasalar.Items.Add(lvi);
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (e.ClickedItem == tsmiUrunler)
            {
                new UrunlerForm().ShowDialog();
            }
            else if (e.ClickedItem == tsmiGecmisSiparisler)
            {
                new GecmisSiparislerForm().ShowDialog();
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
            siparisForm.ShowDialog();

            //Siparis formu kapandiktan sonra siparis durumunu kontrol et
            if (siparis.Durum != SiparisDurum.Aktif)
            {
                lvi.ImageKey = "bos";
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
    }
}
