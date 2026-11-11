# Benzin İstasyonu Simülasyonu

## Depo (Tibet)

Kamyondan gelen koliler yükleme rampasına düşüyor, oyuncu bunları alıp markete taşıyor.

### Kontroller
- **E**: koliyi al (sadece kolinin yanındayken)
- **G**: koliyi önüne bırak
- **E** (düğmenin yanında): kepengi aç/kapat

### Feel notes
- **Otomatik alma yerine E:** yanından geçerken koli kafamıza çıkıyordu, üç koli yan yanayken hangisini alacağımızı seçemiyorduk.
- **G yere koyuyor, yağ fırlatıyor:** G bilerek yapılan bir hareket, o yüzden koli sakin bir şekilde önümüze konuyor. Yağ ise kaza, orada koli arkaya fırlıyor.
- **Put down distance 1.1:** daha azında koli karakterin içine giriyordu, daha fazlasında çok uzağa düşüyordu.
- **Tekrar alma bekleme süresi 0.5 sn:** bırakır bırakmaz yanlışlıkla tekrar alınmasın diye.

### Balance notes
| Ayar | Değer | Neden |
|---|---|---|
| Koli yenilenme süresi | 5 sn | daha azında rampa hiç boşalmıyordu, acele etmeye gerek kalmıyordu |
| Ağır koli ihtimali | %30 | çoğu koli hafif, ağır olan arada geliyor ve fark ediliyor |
| Ağır koli hızı | ×0.55 | yavaşlık hissediliyor ama ceza gibi değil |
| Ağır koli zıplaması | ×0.3 | 1.5 m yerine yaklaşık 0.45 m, sadece küçük bir hop |
| Ağır koli boyutu | ×1.2 | renk tek başına yetmiyor, büyük olunca ağır olduğu anlaşılıyor |

Karakter CharacterController kullandığı için ağırlığı mass ile değil, PlayerMovement'taki SpeedMultiplier ve JumpMultiplier çarpanlarıyla veriyoruz. Normalde 1 oluyorlar, sadece ağır koli taşırken düşüyorlar.

### Playtest
- Oyuncu: ...
- 2 dakikada hafif / ağır: ... / ...
- "Neden onu aldın?": ...
