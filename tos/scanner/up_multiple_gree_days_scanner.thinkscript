# StreakUp (many greens) OR TopReversal (top + X down bars) — Daily or Intraday
# Aggregation:
#   - Daily:    set to Day, usePrevClose = yes  (green = Close > Close[1])
#   - Intraday: set to 5/15-min, usePrevClose = no (green = Close > Open)

################
# USER INPUTS  #
################
input minPrice         = 3.0;
input minAvgVol50      = 300000;        # liquidity guard (useful even intraday)
input consecutiveBars  = 3;             # how many prior GREEN bars define "many"
input usePrevClose     = no;            # no = intraday (Close>Open); yes = daily (Close>Close[1])

# Doji / tiny-body controls
input minBodyBP            = 10;        # body >= X bps of price (1 bp = 0.01%); 0 disables
input useStrictDoji        = yes;       # when yes, also require body as % of range
input minBodyFracOfRange   = 0.15;      # body >= 15% of bar range (only if strict)

# Streak-only extra filter (optional): require a minimum total RISE across the streak
input requireMinRise   = no;
input minRisePct       = 6.0;           # e.g., >= +6% over last N bars

# <<< SIMPLE FLAG TO TOGGLE TOP REVERSAL MODE >>>
input topOn                 = no;       # OFF = green streak; ON = top + X down bars
input topConfirmBars        = 3;        # X = number of RED bars AFTER the top (default 3)
input includeTopInCount     = yes;      # no = require X after the top; yes = count top as 1

################
# CORE LOGIC   #
################
def liquid  = Average(volume, 50) >= minAvgVol50;
def priceOk = close >= minPrice and liquid;

# --- Doji filters ---
def body = AbsValue(close - open);
def rng  = high - low;
def bodyOK_bp    = if minBodyBP <= 0 then yes else ((body / close) * 10000) >= minBodyBP;
def bodyOK_range = (if rng > 0 then body / rng else 0) >= minBodyFracOfRange;
def bodyOK       = if useStrictDoji then (bodyOK_bp and bodyOK_range) else bodyOK_bp;

# Green / Red (both must pass body gate)
def greenByOpen  = close > open      and bodyOK;
def greenByClose = close > close[1]  and bodyOK;
def redByOpen    = close < open      and bodyOK;
def redByClose   = close < close[1]  and bodyOK;

def greenBar = if usePrevClose then greenByClose else greenByOpen;
def redBar   = if usePrevClose then redByClose   else redByOpen;

# --- GREEN STREAK: last N bars are all green (contiguous, ending now)
def lastNAllGreen = Sum(greenBar, consecutiveBars) == consecutiveBars;

# Optional: total % rise across the streak window (close now vs close N bars ago)
def risePct = (close / close[consecutiveBars] - 1) * 100;

# -------------------------
# TOP REVERSAL (top + X red)
# -------------------------
# Track green streak length
rec greenStreak = if greenBar then greenStreak[1] + 1 else 0;

# "Top" = first RED after >= N greens
def firstRedAfterStreak = redBar and greenStreak[1] >= consecutiveBars;

# Confirm X red bars AFTER the top (or include the top bar as #1)
def shift = if includeTopInCount then Max(0, topConfirmBars - 1) else topConfirmBars;
def redsAll = if topConfirmBars <= 0 then yes else Sum(redBar, topConfirmBars) == topConfirmBars;

def topOK = firstRedAfterStreak[shift] and redsAll;

################
# FINAL PLOT   #
################
plot scan =
     priceOk
  and (
        ( !topOn
          and lastNAllGreen
          and (if requireMinRise then risePct >= minRisePct else yes)
        )
     or (  topOn
          and topOK
        )
      );