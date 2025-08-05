# BDH.Core.Line.Console

## Proje Hakkýnda

Bu sistem, hastane ortamýnda kullanýlan, Panates firmasýnýn Abisena isimli HBYS (Hastane Bilgi Yönetim Sistemi) uygulamasý baz alýnarak geliþtirilmiþtir.  
Oracle veritabaný üzerinde SQL, DCN, trigger, Notification, Oracle AQ (Advanced Queuing) ve SQLDependency gibi teknolojiler kullanýlmýþtýr.  
Sistem, baþka otomasyon uygulamalarý için veritabanýnda deðiþiklikler yapýldýðýnda da çalýþabilir ve esnek bir entegrasyon sunar.

## Özellikler

- Gerçek zamanlý hasta çaðrý ve sýra takibi
- SignalR ile canlý veri yayýný
- SSE ile alternatif veri yayýný
- Oracle veritabaný entegrasyonu
- Saðlýk durumu (health check) endpoint'i
- Merkezi konfigürasyon yönetimi (appsettings.json)
- Modern .NET 9 ve C# 13 kod yapýsý

## Kurulum

1. **Baðýmlýlýklar:**  
   - .NET 9 SDK
   - Oracle.ManagedDataAccess.Client NuGet paketi

2. **Veritabaný Ayarlarý:**  
   - `appsettings.json` dosyasýndaki `ConnectionStrings:OracleDb` alanýný kendi veritabaný bilgilerinizle doldurun.
   - Örnek:
     ```
     "ConnectionStrings": {
       "OracleDb": "User Id=KULLANICI;Password=ÞÝFRE;Data Source=IP:PORT/SERVIS"
     }
     ```
     > Not: `appsettings.json` dosyasý hassas bilgiler içerdiði için repoya eklenmemiþtir. Lütfen `appsettings.json.example` dosyasýný kopyalayýp kendi ayarlarýnýzý girerek `appsettings.json` olarak kaydedin.
3. **Projeyi Derleyin ve Çalýþtýrýn:**

4. **Uygulamayý Açýn:**  
- Varsayýlan olarak [http://localhost:1771/queue](http://localhost:1771/queue) adresinden eriþebilirsiniz.

## Konfigürasyon

Tüm ayarlar `appsettings.json` dosyasýndan yönetilir.

## Saðlýk Kontrolü

Uygulamanýn saðlýk durumunu kontrol etmek için:
- [http://localhost:1771/health](http://localhost:1771/health) adresini ziyaret edebilirsiniz. Bu endpoint, uygulamanýn çalýþýr durumda olup olmadýðýný gösterir.
Bu endpoint, veritabaný, SignalR ve SSE servislerinin durumunu JSON olarak döner.

## Katký ve Geliþtirme

- Kodunuzu düzenli ve açýklamalý tutun.
- Yeni özellikler için branch açarak katkýda bulunun.
- Pull request göndermeden önce kodun derlendiðinden ve test edildiðinden emin olun.

## Lisans

Bu proje MIT lisansý ile yayýnlanmýþtýr.  
Daha fazla bilgi için `LICENSE` dosyasýna bakýnýz.

---

**Not:**  
Gerçek veritabaný þifreleri ve hassas bilgileri paylaþmayýnýz.  
Sorularýnýz veya katkýlarýnýz için GitHub Issues üzerinden iletiþime geçebilirsiniz.