using AnkaKafe.Data;
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
    public partial class SiparisForm : Form
    {
        
        public event EventHandler<MasaTasindiEventArgs> MasaTasindi;

        private readonly KafeVeri _db;
        private readonly Siparis _siparis;
        private readonly BindingList<SiparisDetay> _blSiparisDetaylar;

        public SiparisForm(KafeVeri kafeVeri, Siparis siparis)
        {
            _db = kafeVeri;
            _siparis = siparis;
            _blSiparisDetaylar = new BindingList<SiparisDetay>(siparis.SiparisDetaylar);
            InitializeComponent();
            dgvSiparisDetaylar.AutoGenerateColumns = false;//otomatik sutun olusturmayi kapat
            UrunleriGoster();
            EkleFormSifirla();
            MasaNoGuncelle();
            FiyatGuncelle();
            DetaylariListele();
            MasaNolariDoldur();
            _blSiparisDetaylar.ListChanged += _blSiparisDetaylar_ListChanged;
        }

        private void MasaNolariDoldur()
        {
            List<int> bosMasaNolar = new List<int>();
            for (int i = 1; i <= _db.MasaAdet; i++)
            {
                if (!_db.AktifSiparisler.Any(x => x.MasaNo == i))
                {
                    bosMasaNolar.Add(i);
                }
            }

            cboMasaNo.DataSource = bosMasaNolar;


            // todo: cboMasaNo.DataSource = Enumerable.Range(1,20).Where(i => _db.MasaAdet)
        }

        private void _blSiparisDetaylar_ListChanged(object sender, ListChangedEventArgs e)
        {
            FiyatGuncelle();
        }

        private void UrunleriGoster()
        {
            cboUrun.DataSource = _db.Urunler;
        }

        private void FiyatGuncelle()
        {
            lblOdemeTutar.Text = _siparis.ToplamTutarTL;
        }

        private void MasaNoGuncelle()
        {
            Text = $"Masa {_siparis.MasaNo} Siparis Bilgileri";
            lblMasaNo.Text = _siparis.MasaNo.ToString("00");
        }

        private void btnEkle_Click(object sender, EventArgs e)
        {
            if (cboUrun.SelectedIndex == -1 || nudAdet.Value < 1) 
                return;//secilen urun yok, metotdan cik


            Urun urun = (Urun)cboUrun.SelectedItem;

            SiparisDetay mevcut = _siparis.SiparisDetaylar.FirstOrDefault(x => x.UrunAd == urun.UrunAd);
            if (mevcut == null)
            {
            SiparisDetay siparisDetay = new SiparisDetay()
            {
                UrunAd = urun.UrunAd,
                BirimFiyat = urun.BirimFiyat,
                Adet = (int)nudAdet.Value
            };

            _blSiparisDetaylar.Add(siparisDetay);

            }
            else
            {
                mevcut.Adet += (int)nudAdet.Value;
                _blSiparisDetaylar.ResetBindings(); 
            }


            EkleFormSifirla();
        }

        private void EkleFormSifirla()
        {
            cboUrun.SelectedIndex = -1;//en son urun secili kalsin istersek yoruma alalim
            nudAdet.Value = 1;
        }

        private void DetaylariListele()
        {
            dgvSiparisDetaylar.DataSource = _blSiparisDetaylar;

        }

        private void dgvSiparisDetaylar_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult dr = MessageBox.Show(
                text: "Secili Siparis Detaylari Silinecektir. Onayliyor musun?",
                caption: "Silme Onayi",
                buttons: MessageBoxButtons.YesNo,
                icon: MessageBoxIcon.Exclamation,
                defaultButton: MessageBoxDefaultButton.Button2);

            //true atamaniz sonucunda satir silme isleminin onune gecmis olursunuz.
            e.Cancel = dr == DialogResult.No;
        }

        private void btnAnasayfa_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnIptal_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Iptal,0);
        }

        private void btnOde_Click(object sender, EventArgs e)
        {
            SiparisKapat(SiparisDurum.Odendi,_siparis.ToplamTutar());
        }

        private void SiparisKapat(SiparisDurum siparisDurum,decimal odenenTutar)
        {
            _siparis.OdenenTutar = odenenTutar;
            _siparis.Durum = siparisDurum;
            _siparis.KapanisZamani = DateTime.Now;
            _db.AktifSiparisler.Remove(_siparis);
            _db.GecmisSiparis.Add(_siparis);
            Close();
        }

        private void btnTasi_Click(object sender, EventArgs e)
        {
            if (cboMasaNo.SelectedIndex == -1) return;

            int eskiMasaNo = _siparis.MasaNo;
            int yeniMasaNo = (int)cboMasaNo.SelectedItem;
            _siparis.MasaNo = yeniMasaNo;
            MasaNolariDoldur();//dolu masalar degisti.

            //kisayolu
            MasaTasindi?.Invoke(this, new MasaTasindiEventArgs(eskiMasaNo, yeniMasaNo));
            /*if (MasaTasindi != null)
            {
                MasaTasindi(this, new MasaTasindiEventArgs(eskiMasaNo, yeniMasaNo));
            }*/


            MasaNoGuncelle();
        }
    }
}
