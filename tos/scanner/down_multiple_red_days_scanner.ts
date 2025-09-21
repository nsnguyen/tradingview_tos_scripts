# Streak (many reds) OR Reversal (bottom + X up bars) — Daily or Intraday
# Aggregation:
#   - Daily:    set to Day, usePrevClose = yes  (red = Close < Close[1])
#   - Intraday: set to 5/15-min, usePrevClose = no (red = Close < Open)

################
# USER INPUTS  #
################
input minPrice         = 3.0;
input minAvgVol50      = 300000;        # liquidity guard (useful even intraday)
input consecutiveBars  = 3;             # how many prior reds define "many"
input usePrevClose     = yes;           

# Doji / tiny-body controls
input minBodyBP            = 10;        # body >= X bps of price (1 bp = 0.01%); 0 disables
input useStrictDoji        = no;        # when yes, also require body as % of range
input minBodyFracOfRange   = 0.15;      # body >= 15% of bar range (used only if strict)

# Streak-only extra filter (optional)
input requireMinDrop   = no;
input minDropPct       = -6.0;          # e.g., <= -6% over last N bars

# <<< SIMPLE FLAG TO TOGGLE REVERSAL MODE >>>
input reversalOn             = no;      # OFF = streak; ON = bottom + X up bars
input reversalConfirmBars    = 3;       # X = number of up bars AFTER bottom (default 3)
input includeBottomInCount   = no;      # no = require X bars AFTER the bottom; yes = count bottom as 1

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

# Red / Green (both must pass body gate)
def redByOpen    = close < open      and bodyOK;
def redByClose   = close < close[1]  and bodyOK;
def greenByOpen  = close > open      and bodyOK;
def greenByClose = close > close[1]  and bodyOK;

def redBar   = if usePrevClose then redByClose   else redByOpen;
def greenBar = if usePrevClose then greenByClose else greenByOpen;

# Streak: last N bars are all red (contiguous, ending now)
def lastNAllRed = Sum(redBar, consecutiveBars) == consecutiveBars;

# Optional: total % drop across the streak window (close now vs close N bars ago)
def dropPct = (close / close[consecutiveBars] - 1) * 100;

# -------------------------
# REVERSAL (bottom + X up)
# -------------------------
# Bottom = first green after >= N reds
rec redStreak = if redBar then redStreak[1] + 1 else 0;
def firstGreenAfterStreak = greenBar and redStreak[1] >= consecutiveBars;

# Confirm X green bars AFTER the bottom.
# If includeBottomInCount = yes, count the bottom bar as one of the greens.
def shift = if includeBottomInCount then Max(0, reversalConfirmBars - 1) else reversalConfirmBars;
def greensAll = if reversalConfirmBars <= 0 then yes else Sum(greenBar, reversalConfirmBars) == reversalConfirmBars;

def reversalOK = firstGreenAfterStreak[shift] and greensAll;

################
# FINAL PLOT   #
################
plot scan =
     priceOk
  and (
        ( !reversalOn
          and lastNAllRed
          and (if requireMinDrop then dropPct <= minDropPct else yes)
        )
     or (  reversalOn
          and reversalOK
        )
      );