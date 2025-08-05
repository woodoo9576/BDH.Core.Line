# BDH.Core.Line.Console

## Proje Hakk�nda

Bu sistem, hastane ortam�nda kullan�lan, Panates firmas�n�n Abisena isimli HBYS (Hastane Bilgi Y�netim Sistemi) uygulamas� baz al�narak geli�tirilmi�tir.  
Oracle veritaban� �zerinde SQL, DCN, trigger, Notification, Oracle AQ (Advanced Queuing) ve SQLDependency gibi teknolojiler kullan�lm��t�r.  
Sistem, ba�ka otomasyon uygulamalar� i�in veritaban�nda de�i�iklikler yap�ld���nda da �al��abilir ve esnek bir entegrasyon sunar.

## �zellikler

- Ger�ek zamanl� hasta �a�r� ve s�ra takibi
- SignalR ile canl� veri yay�n�
- SSE ile alternatif veri yay�n�
- Oracle veritaban� entegrasyonu
- Sa�l�k durumu (health check) endpoint'i
- Merkezi konfig�rasyon y�netimi (appsettings.json)
- Modern .NET 9 ve C# 13 kod yap�s�

## Kurulum

1. **Ba��ml�l�klar:**  
   - .NET 9 SDK
   - Oracle.ManagedDataAccess.Client NuGet paketi

2. **Veritaban� Ayarlar�:**  
   - `appsettings.json` dosyas�ndaki `ConnectionStrings:OracleDb` alan�n� kendi veritaban� bilgilerinizle doldurun.
   - �rnek:
     ```
     "ConnectionStrings": {
       "OracleDb": "User Id=KULLANICI;Password=��FRE;Data Source=IP:PORT/SERVIS"
     }
     ```
     > Not: `appsettings.json` dosyas� hassas bilgiler i�erdi�i i�in repoya eklenmemi�tir. L�tfen `appsettings.json.example` dosyas�n� kopyalay�p kendi ayarlar�n�z� girerek `appsettings.json` olarak kaydedin.
3. **Projeyi Derleyin ve �al��t�r�n:**

4. **Uygulamay� A��n:**  
- Varsay�lan olarak [http://localhost:1771/queue](http://localhost:1771/queue) adresinden eri�ebilirsiniz.

## Konfig�rasyon

T�m ayarlar `appsettings.json` dosyas�ndan y�netilir.

## Sa�l�k Kontrol�

Uygulaman�n sa�l�k durumunu kontrol etmek i�in:
- [http://localhost:1771/health](http://localhost:1771/health) adresini ziyaret edebilirsiniz. Bu endpoint, uygulaman�n �al���r durumda olup olmad���n� g�sterir.
Bu endpoint, veritaban�, SignalR ve SSE servislerinin durumunu JSON olarak d�ner.

## Katk� ve Geli�tirme

- Kodunuzu d�zenli ve a��klamal� tutun.
- Yeni �zellikler i�in branch a�arak katk�da bulunun.
- Pull request g�ndermeden �nce kodun derlendi�inden ve test edildi�inden emin olun.

## Lisans

Bu proje MIT lisans� ile yay�nlanm��t�r.  
Daha fazla bilgi i�in `LICENSE` dosyas�na bak�n�z.

---

**Not:**  
Ger�ek veritaban� �ifreleri ve hassas bilgileri payla�may�n�z.  
Sorular�n�z veya katk�lar�n�z i�in GitHub Issues �zerinden ileti�ime ge�ebilirsiniz.