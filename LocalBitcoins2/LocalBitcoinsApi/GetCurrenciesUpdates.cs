using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalBitcoins
{
    public class GetCurrenciesUpdates
    {
        public class RatesCOP
        {
            public string last { get; set; }
        }

        public class COP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesCOP rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesUSD
        {
            public string last { get; set; }
        }

        public class USD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesUSD rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesTWD
        {
            public string last { get; set; }
        }

        public class TWD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesTWD rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesGHS
        {
            public string last { get; set; }
        }

        public class GHS
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesGHS rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesDASH
        {
            public string last { get; set; }
        }

        public class DASH
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesDASH rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesNGN
        {
            public string last { get; set; }
        }

        public class NGN
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesNGN rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesEGP
        {
            public string last { get; set; }
        }

        public class EGP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesEGP rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesIDR
        {
            public string last { get; set; }
        }

        public class IDR
        {
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesIDR rates { get; set; }
        }

        public class RatesHNL
        {
            public string last { get; set; }
        }

        public class HNL
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesHNL rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesISK
        {
            public string last { get; set; }
        }

        public class ISK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesISK rates { get; set; }
        }

        public class RatesCRC
        {
            public string last { get; set; }
        }

        public class CRC
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesCRC rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesPAB
        {
            public string last { get; set; }
        }

        public class PAB
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesPAB rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesAED
        {
            public string last { get; set; }
        }

        public class AED
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesAED rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesGBP
        {
            public string last { get; set; }
        }

        public class GBP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesGBP rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesDOP
        {
            public string last { get; set; }
        }

        public class DOP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesDOP rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesCZK
        {
            public string last { get; set; }
        }

        public class CZK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesCZK rates { get; set; }
        }

        public class RatesLKR
        {
            public string last { get; set; }
        }

        public class LKR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesLKR rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesETH
        {
            public string last { get; set; }
        }

        public class ETH
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesETH rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesRSD
        {
            public string last { get; set; }
        }

        public class RSD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesRSD rates { get; set; }
        }

        public class RatesCAD
        {
            public string last { get; set; }
        }

        public class CAD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesCAD rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesPKR
        {
            public string last { get; set; }
        }

        public class PKR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesPKR rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesJPY
        {
            public string last { get; set; }
        }

        public class JPY
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesJPY rates { get; set; }
        }

        public class RatesTZS
        {
            public string last { get; set; }
        }

        public class TZS
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesTZS rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesVND
        {
            public string last { get; set; }
        }

        public class VND
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesVND rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesRON
        {
            public string last { get; set; }
        }

        public class RON
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesRON rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesHUF
        {
            public string last { get; set; }
        }

        public class HUF
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public RatesHUF rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesTRY
        {
            public string last { get; set; }
        }

        public class TRY
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesTRY rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class RatesMYR
        {
            public string last { get; set; }
        }

        public class MYR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesMYR rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates29
        {
            public string last { get; set; }
        }

        public class JMD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates29 rates { get; set; }
        }

        public class Rates30
        {
            public string last { get; set; }
        }

        public class XRP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates30 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates31
        {
            public string last { get; set; }
        }

        public class BAM
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates31 rates { get; set; }
        }

        public class Rates32
        {
            public string last { get; set; }
        }

        public class UAH
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates32 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates33
        {
            public string last { get; set; }
        }

        public class UGX
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates33 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates34
        {
            public string last { get; set; }
        }

        public class ZMW
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates34 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates35
        {
            public string last { get; set; }
        }

        public class SAR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates35 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates36
        {
            public string last { get; set; }
        }

        public class DKK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates36 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates37
        {
            public string last { get; set; }
        }

        public class SEK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates37 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates38
        {
            public string last { get; set; }
        }

        public class SGD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates38 rates { get; set; }
        }

        public class Rates39
        {
            public string last { get; set; }
        }

        public class HKD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates39 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates40
        {
            public string last { get; set; }
        }

        public class GEL
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates40 rates { get; set; }
        }

        public class RatesAUD
        {
            public string last { get; set; }
        }

        public class AUD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public RatesAUD rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates42
        {
            public string last { get; set; }
        }

        public class CHF
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates42 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates43
        {
            public string last { get; set; }
        }

        public class IRR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates43 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates44
        {
            public string last { get; set; }
        }

        public class KRW
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates44 rates { get; set; }
        }

        public class Rates45
        {
            public string last { get; set; }
        }

        public class CNY
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates45 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates46
        {
            public string last { get; set; }
        }

        public class VEF
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates46 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates47
        {
            public string last { get; set; }
        }

        public class BDT
        {
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates47 rates { get; set; }
        }

        public class Rates48
        {
            public string last { get; set; }
        }

        public class TTD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates48 rates { get; set; }
        }

        public class Rates49
        {
            public string last { get; set; }
        }

        public class HRK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates49 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates50
        {
            public string last { get; set; }
        }

        public class NZD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates50 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates51
        {
            public string last { get; set; }
        }

        public class BYN
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates51 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates52
        {
            public string last { get; set; }
        }

        public class CLP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates52 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates53
        {
            public string last { get; set; }
        }

        public class THB
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates53 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates54
        {
            public string last { get; set; }
        }

        public class XAF
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates54 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates55
        {
            public string last { get; set; }
        }

        public class EUR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates55 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates56
        {
            public string last { get; set; }
        }

        public class LTC
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates56 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates57
        {
            public string last { get; set; }
        }

        public class ARS
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates57 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates58
        {
            public string last { get; set; }
        }

        public class UYU
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates58 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates59
        {
            public string last { get; set; }
        }

        public class KZT
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates59 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates60
        {
            public string last { get; set; }
        }

        public class NOK
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates60 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates61
        {
            public string last { get; set; }
        }

        public class RUB
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates61 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates62
        {
            public string last { get; set; }
        }

        public class KES
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates62 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates63
        {
            public string last { get; set; }
        }

        public class PEN
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates63 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates64
        {
            public string last { get; set; }
        }

        public class INR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates64 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates65
        {
            public string last { get; set; }
        }

        public class MXN
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates65 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates66
        {
            public string last { get; set; }
        }

        public class OMR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates66 rates { get; set; }
        }

        public class Rates67
        {
            public string last { get; set; }
        }

        public class BRL
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates67 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates68
        {
            public string last { get; set; }
        }

        public class MAD
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates68 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates69
        {
            public string last { get; set; }
        }

        public class PLN
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates69 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates70
        {
            public string last { get; set; }
        }

        public class PHP
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates70 rates { get; set; }
        }

        public class Rates71
        {
            public string last { get; set; }
        }

        public class ZAR
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public string avg_1h { get; set; }
            public Rates71 rates { get; set; }
            public string avg_6h { get; set; }
        }

        public class Rates72
        {
            public string last { get; set; }
        }

        public class ILS
        {
            public string avg_12h { get; set; }
            public string volume_btc { get; set; }
            public string avg_24h { get; set; }
            public Rates72 rates { get; set; }
        }

        public class RootGetCurrency
        {
            public COP COP { get; set; }
            public USD USD { get; set; }
            public TWD TWD { get; set; }
            public GHS GHS { get; set; }
            public DASH DASH { get; set; }
            public NGN NGN { get; set; }
            public EGP EGP { get; set; }
            public IDR IDR { get; set; }
            public HNL HNL { get; set; }
            public ISK ISK { get; set; }
            public CRC CRC { get; set; }
            public PAB PAB { get; set; }
            public AED AED { get; set; }
            public GBP GBP { get; set; }
            public DOP DOP { get; set; }
            public CZK CZK { get; set; }
            public LKR LKR { get; set; }
            public ETH ETH { get; set; }
            public RSD RSD { get; set; }
            public CAD CAD { get; set; }
            public PKR PKR { get; set; }
            public JPY JPY { get; set; }
            public TZS TZS { get; set; }
            public VND VND { get; set; }
            public RON RON { get; set; }
            public HUF HUF { get; set; }
            public TRY TRY { get; set; }
            public MYR MYR { get; set; }
            public JMD JMD { get; set; }
            public XRP XRP { get; set; }
            public BAM BAM { get; set; }
            public UAH UAH { get; set; }
            public UGX UGX { get; set; }
            public ZMW ZMW { get; set; }
            public SAR SAR { get; set; }
            public DKK DKK { get; set; }
            public SEK SEK { get; set; }
            public SGD SGD { get; set; }
            public HKD HKD { get; set; }
            public GEL GEL { get; set; }
            public AUD AUD { get; set; }
            public CHF CHF { get; set; }
            public IRR IRR { get; set; }
            public KRW KRW { get; set; }
            public CNY CNY { get; set; }
            public VEF VEF { get; set; }
            public BDT BDT { get; set; }
            public TTD TTD { get; set; }
            public HRK HRK { get; set; }
            public NZD NZD { get; set; }
            public BYN BYN { get; set; }
            public CLP CLP { get; set; }
            public THB THB { get; set; }
            public XAF XAF { get; set; }
            public EUR EUR { get; set; }
            public LTC LTC { get; set; }
            public ARS ARS { get; set; }
            public UYU UYU { get; set; }
            public KZT KZT { get; set; }
            public NOK NOK { get; set; }
            public RUB RUB { get; set; }
            public KES KES { get; set; }
            public PEN PEN { get; set; }
            public INR INR { get; set; }
            public MXN MXN { get; set; }
            public OMR OMR { get; set; }
            public BRL BRL { get; set; }
            public MAD MAD { get; set; }
            public PLN PLN { get; set; }
            public PHP PHP { get; set; }
            public ZAR ZAR { get; set; }
            public ILS ILS { get; set; }
        }
    }
}
