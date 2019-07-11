# FMUSI2019_BNFParser
Ne toliko losa implementacija Earley algoritma za parsiranje teksta po zadatom BNF-u.

# Radi
* Sve trazene ekstenzije u PDFu zadatka
* Rekurzivni BNF, nebitno gdje se nalazi "rekurzivni" token
* Oslanja se na to da ce kljucevi dodati u Dictionary<T1,T2> biti dodati na kraj S.Keys, ako se to ikad promijeni ovo prestaje da radi.

# Ne radi
* Patterni gdje bi regex(...) uhvatio dio koji se nalazi u drugom sljedecem tokenu npr

  \<a\> ::= regex(a*) "a"
  
  nikad nece biti prepoznat.
