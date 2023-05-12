suc(0).
suc(suc(T)) :- suc(T).

taro.

a(0).
a(a(0)).
a(a(a(0))).

b(0,1).
b(1,1).

even(0).
even(0).
even(suc(suc(T))) :- even(T).

odd(suc(0)).
odd(suc(suc(T))) :- odd(T).

w(0) :- write("kore ha 0").
w(1) :- write("1 jan").
w(2) :- nl.
