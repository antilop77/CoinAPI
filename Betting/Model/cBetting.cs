namespace CoinAPI.Betting.Model
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<List<cBetting>>(myJsonResponse);
    public class BothToScore
    {
        public Yes? yes { get; set; }
        public No? no { get; set; }
    }

    public class Half
    {
    }

    public class Handicaps1
    {
        public double type { get; set; }
        public double v { get; set; }
    }

    public class Handicaps2
    {
        public double type { get; set; }
        public double v { get; set; }
    }

    public class League
    {
        public int country_id { get; set; }
        public int league_id { get; set; }
        public int sport_id { get; set; }
        public string? name { get; set; }
        public string? name_rus { get; set; }
        public bool isCyber { get; set; }
        public bool isSimulated { get; set; }
        public bool isSpecial { get; set; }
    }

    public class Markets
    {
        public List<Total>? totals { get; set; }
        public List<Totals1>? totals1 { get; set; }
        public List<Totals2>? totals2 { get; set; }
        public List<TotalsAsian>? totalsAsian { get; set; }
        public List<Totals1Asian>? totals1Asian { get; set; }
        public List<Totals2Asian>? totals2Asian { get; set; }
        public List<Handicaps1>? handicaps1 { get; set; }
        public List<Handicaps2>? handicaps2 { get; set; }
        public BothToScore? bothToScore { get; set; }
        public Half? half { get; set; }
        public Win1? win1 { get; set; }
        public WinX? winX { get; set; }
        public Win2? win2 { get; set; }
        public Win1X? win1X { get; set; }
        public Win12? win12 { get; set; }
        public WinX2? winX2 { get; set; }
    }

    public class No
    {
        public double v { get; set; }
    }

    public class Over
    {
        public double v { get; set; }
    }

    public class cBetting
    {
        public int v { get; set; }
        public int id { get; set; }
        public string? team1 { get; set; }
        public string? team1_rus { get; set; }
        public int team1_id { get; set; }
        public string? team2 { get; set; }
        public string? team2_rus { get; set; }
        public int team2_id { get; set; }
        public int score1 { get; set; }
        public int score2 { get; set; }
        public string? href { get; set; }
        public DateTime date_start { get; set; }
        public bool isLive { get; set; }
        public bool isComposite { get; set; }
        public bool isSpecial { get; set; }
        public int minute { get; set; }
        public int seconds { get; set; }
        public int half_order_index { get; set; }
        public string? title { get; set; }
        public string? country { get; set; }
        public League? league { get; set; }
        public Markets? markets { get; set; }
        public string? hash { get; set; }
        public DateTime? actual_at { get; set; }
        public Stats? stats { get; set; }
    }

    public class Stats
    {
        public int attacks1 { get; set; }
        public int attacks2 { get; set; }
        public int attacks_danger_1 { get; set; }
        public int attacks_danger_2 { get; set; }
        public int possession1 { get; set; }
        public int possession2 { get; set; }
        public int shoots_on1 { get; set; }
        public int shoots_on2 { get; set; }
        public int shoots_off1 { get; set; }
        public int shoots_off2 { get; set; }
        public int yellow_cards1 { get; set; }
        public int yellow_cards2 { get; set; }
        public int corners1 { get; set; }
        public int corners2 { get; set; }
        public int red_cards1 { get; set; }
        public int red_cards2 { get; set; }
        public int penalty_1 { get; set; }
        public int penalty_2 { get; set; }
        public int substitutions1 { get; set; }
        public int substitutions2 { get; set; }
    }

    public class Total
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class Totals1
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class Totals1Asian
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class Totals2
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class Totals2Asian
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class TotalsAsian
    {
        public double type { get; set; }
        public Over? over { get; set; }
        public Under? under { get; set; }
    }

    public class Under
    {
        public double v { get; set; }
    }

    public class Win1
    {
        public double v { get; set; }
    }

    public class Win12
    {
        public double v { get; set; }
    }

    public class Win1X
    {
        public double v { get; set; }
    }

    public class Win2
    {
        public double v { get; set; }
    }

    public class WinX
    {
        public double v { get; set; }
    }

    public class WinX2
    {
        public double v { get; set; }
    }

    public class Yes
    {
        public double v { get; set; }
    }
}
