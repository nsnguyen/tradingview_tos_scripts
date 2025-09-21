... R R R R R R  G  G  G ...
               ^  ^  ^
               |  |  └─ triggers when set to ThreeUp
               |  └──── triggers when set to TwoUp
               └─────── triggers when set to FirstGreen

A) includeBottomInCount = no (bottom doesn’t count)
You need 3 greens after the bottom → triggers on the 3rd green after.
... R R R  G  G  G  G ...
           ^  ^  ^  ^
           |  |  |  └─ scanner fires here (3 after bottom)
           |  |  └─── 2 after
           |  └────── 1 after
           └───────── bottom (not counted)


B) includeBottomInCount = yes (bottom does count)
Bottom counts as #1 → you need 2 more greens → triggers on the 2nd green after.
... R R R  G  G  G ...
           ^  ^  ^
           |  |  └─ scanner fires here (bottom=1, then +2)
           |  └─── +1 after
           └────── bottom (counts as #1)