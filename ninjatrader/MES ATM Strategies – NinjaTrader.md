# MES ATM Strategies – NinjaTrader

This document summarizes the MES (Micro E-mini S&P 500) ATM templates used for fast scalps and a runner with AutoTrail logic.  
All examples assume **1 MES contract**.

## 1. Core MES Info

| Item            | Value                              |
|-----------------|------------------------------------|
| Instrument      | MES                                |
| Tick size       | 0.25 points                        |
| Tick value      | $1.25 per tick (per contract)      |
| Points-to-ticks | 1 point = 4 ticks = $5.00 contract |

## 2. ATM #1 – Fast Scalp (Breakeven Only)

**Goal**: Quick scalp with small stop, modest target, and a simple breakeven bump.  
**Template name**: `MES_SCALP_5SL_8TP_BE4+1`

**ATM settings**
- Order qty: 1  
- TIF: DAY  
- Parameter type: Ticks  
- Stop loss: 5 ticks (≈ $6.25 risk)  
- Profit target: 8 ticks (≈ $10.00 reward)

**Stop strategy – Breakeven**
1. `Stop strategy → Custom…`
2. Enable Auto breakeven
3. Profit trigger: 4 ticks
4. Plus: +1 tick

**Behavior (long example)**
- Initial stop sits 5 ticks below entry.  
- When trade reaches +4 ticks, stop jumps to entry +1 tick, locking a small gain.  
- No AutoTrail; it’s intentionally simple and fast.

## 3. ATM #2 – Ultra Tight Scalper

**Goal**: Extremely tight scalp for precision entries.  
**Template name**: `MES_ULTRA_3SL_4TP_BE3`

**ATM settings**
- Order qty: 1  
- TIF: DAY  
- Parameter type: Ticks  
- Stop loss: 3 ticks (≈ $3.75 risk)  
- Profit target: 4 ticks (≈ $5.00 reward)

**Stop strategy – Breakeven**
1. Enable Auto breakeven
2. Profit trigger: 3 ticks
3. Plus: 0 ticks (true breakeven)

**Behavior (long example)**
- Initial stop starts 3 ticks below entry.  
- At +3 ticks, stop moves to entry price.  
- Target rests at +4 ticks.  
- No AutoTrail; it’s a “hit and run” template.

## 4. ATM #3 – Runner With AutoTrail

**Goal**: Capture larger moves while tightening risk once the trade works.  
**Template name**: `MES_RUNNER_6SL_16TP_TRAIL`

**ATM settings**
- Order qty: 1  
- TIF: DAY  
- Parameter type: Ticks  
- Stop loss: 6 ticks (≈ $7.50 risk)  
- Profit target: 16 ticks (≈ $20.00 reward)

**Stop strategy – Breakeven + AutoTrail**

*Auto Breakeven*
1. Enable Auto breakeven  
2. Profit trigger: 8 ticks  
3. Plus: +1 tick  
4. Result: once price hits +8 ticks, stop shifts from -6 ticks to entry +1 to lock a small gain.

*AutoTrail (first row)*
1. Profit trigger: 12 ticks  
2. Stop-loss offset: 6 ticks  
3. Frequency: 2 ticks

**Behavior (long example)**
- Entry: 6000.00 → initial stop at 5998.50.  
- +8 ticks (6002.00): stop jumps to 6000.25 (breakeven +1).  
- +12 ticks (6003.00): AutoTrail activates, stop follows 6 ticks behind (6001.50).  
- Every additional +2 ticks raises the stop another 2 ticks, keeping distance ≈6 ticks.  
- +16 ticks (6004.00): either the fixed target fills or you let the trailer keep following.

## 5. Save and Use the Templates

**Save**
1. Open the ATM Strategy window in NinjaTrader.  
2. Enter the parameters shown above.  
3. Click “Save as template”.  
4. Use these names:  
   - `MES_SCALP_5SL_8TP_BE4+1`  
   - `MES_ULTRA_3SL_4TP_BE3`  
   - `MES_RUNNER_6SL_16TP_TRAIL`  
5. Click OK.

**Use**
1. In Chart Trader or SuperDOM, open the ATM Strategy dropdown.  
2. Pick the template needed:  
   - Fast scalp → `MES_SCALP_5SL_8TP_BE4+1`  
   - Ultra tight hits → `MES_ULTRA_3SL_4TP_BE3`  
   - Bigger move / runner → `MES_RUNNER_6SL_16TP_TRAIL`  
3. Place the entry order (market, limit, stop-limit, etc.).  
4. NinjaTrader sends the linked OCO stop and target and manages the breakeven/AutoTrail rules automatically.

## 6. Risk Notes (Important)

- Account size currently ≈ $100, so even 3–6 tick stops consume 3.75–7.50% of equity.  
- Trade these templates on SIM until you’re consistent and understand the breakeven/AutoTrail behavior live.  
- Daily risk suggestion: limit yourself to 1–2 full-stop losses per session.  
- If either limit hits, step away for the rest of the session.