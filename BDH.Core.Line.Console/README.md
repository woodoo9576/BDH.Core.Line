# BDH.Core.Line.Console

## Proje Hakkında

Bu sistem, hastane ortamında kullanılan, Panates firmasının Abisena isimli HBYS (Hastane Bilgi Yönetim Sistemi) uygulaması baz alınarak geliştirilmiştir.  
Oracle veritabanı üzerinde SQL, DCN, trigger, Notification, Oracle AQ (Advanced Queuing) ve SQLDependency gibi teknolojiler kullanılmıştır.  
Sistem, başka otomasyon uygulamaları için veritabanında değişiklikler yapıldığında da çalışabilir ve esnek bir entegrasyon sunar.

## Özellikler

- Gerçek zamanlı hasta çağrı ve sıra takibi
- SignalR ile canlı veri yayını
- SSE ile alternatif veri yayını
- Oracle veritabanı entegrasyonu
- Sağlık durumu (health check) endpoint'i
- Merkezi konfigürasyon yönetimi (appsettings.json)
- Modern .NET 9 ve C# 13 kod yapısı

## Kurulum

1. **Bağımlılıklar:**  
   - .NET 9 SDK
   - Oracle.ManagedDataAccess.Client NuGet paketi

2. **Veritabanı Ayarları:**  
   - `appsettings.json` dosyasındaki `ConnectionStrings:OracleDb` alanını kendi veritabanı bilgilerinizle doldurun.
   - Örnek:
     ```
     "ConnectionStrings": {
       "OracleDb": "User Id=KULLANICI;Password=ŞİFRE;Data Source=IP:PORT/SERVIS"
     }
     ```
     > Not: `appsettings.json` dosyası hassas bilgiler içerdiği için repoya eklenmemiştir. Lütfen `appsettings.json.example` dosyasını kopyalayıp kendi ayarlarınızı girerek `appsettings.json` olarak kaydedin.
3. **Projeyi Derleyin ve Çalıştırın:**

4. **Uygulamayı Açın:**  
- Varsayılan olarak [http://localhost:1771/queue](http://localhost:1771/queue) adresinden erişebilirsiniz.

## Konfigürasyon

Tüm ayarlar `appsettings.json` dosyasından yönetilir.

## Sağlık Kontrolü

Uygulamanın sağlık durumunu kontrol etmek için:
- [http://localhost:1771/health](http://localhost:1771/health) adresini ziyaret edebilirsiniz. Bu endpoint, uygulamanın çalışır durumda olup olmadığını gösterir.
Bu endpoint, veritabanı, SignalR ve SSE servislerinin durumunu JSON olarak döner.

## Katkı ve Geliştirme

- Kodunuzu düzenli ve açıklamalı tutun.
- Yeni özellikler için branch açarak katkıda bulunun.
- Pull request göndermeden önce kodun derlendiğinden ve test edildiğinden emin olun.

## Lisans

Bu proje Creative Commons Attribution-NonCommercial 4.0 International (CC BY-NC 4.0) lisansı ile yayınlanmıştır.  
Daha fazla bilgi için `LICENSE` dosyasına bakınız.

---

**Not:**  
Gerçek veritabanı şifreleri ve hassas bilgileri paylaşmayınız.  
Sorularınız veya katkılarınız için GitHub Issues üzerinden iletişime geçebilirsiniz.
