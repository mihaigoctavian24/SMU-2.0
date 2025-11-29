# Politica de Securitate

## Versiuni Suportate

| Versiune | Suportată          |
| -------- | ------------------ |
| 1.x.x    | :white_check_mark: |
| < 1.0    | :x:                |

## Raportarea unei Vulnerabilități

Luăm securitatea sistemului nostru în serios. Dacă ai descoperit o vulnerabilitate de securitate, te rugăm să ne informezi responsabil.

### Cum să Raportezi

**⚠️ NU raporta vulnerabilitățile de securitate prin GitHub Issues publice.**

În schimb, trimite un email la: **security@your-domain.com**

### Ce să Incluzi în Raport

- Tipul vulnerabilității (e.g., SQL injection, XSS, autentificare bypass)
- Pași detaliați pentru reproducerea vulnerabilității
- Impactul potențial al vulnerabilității
- Orice PoC (Proof of Concept) sau capturi de ecran relevante
- Sugestii pentru remediere (opțional)

### Procesul Nostru

1. **Confirmare** - Vom confirma primirea raportului în 48 de ore
2. **Evaluare** - Vom evalua severitatea și impactul în 7 zile
3. **Remediere** - Vom lucra la o soluție și vom comunica timeline-ul
4. **Disclosure** - Vom coordona cu tine divulgarea publică după remediere

### Angajamente

- Nu vom iniția acțiuni legale împotriva cercetătorilor de securitate care raportează responsabil
- Vom lucra cu tine pentru a înțelege și rezolva problema
- Vom recunoaște contribuția ta (cu permisiunea ta) în change log-ul de securitate

### Măsuri de Securitate Implementate

#### Autentificare & Autorizare
- JWT tokens cu expirare (Supabase Auth)
- Role-Based Access Control (RBAC)
- Row Level Security (RLS) în baza de date
- Multi-Factor Authentication (opțional)

#### Protecție Date
- Criptare în tranzit (TLS 1.3)
- Criptare în repaus (Supabase managed)
- Validare input pe toate endpoint-urile
- Parametrizare queries SQL

#### Logging & Monitoring
- Audit log pentru acțiuni critice
- Rate limiting per IP
- Alertare pentru comportament suspect

#### Best Practices
- Dependency scanning automat
- Security headers (HSTS, CSP, X-Frame-Options)
- Regular security updates

## Vulnerability Disclosure Timeline

| Severitate | Timeline Țintă |
|------------|----------------|
| Critică    | 24-48 ore      |
| Înaltă     | 7 zile         |
| Medie      | 30 zile        |
| Scăzută    | 90 zile        |

## Hall of Fame

Mulțumim cercetătorilor de securitate care au contribuit la îmbunătățirea securității sistemului nostru:

*Lista va fi actualizată pe măsură ce primim rapoarte.*

---

Mulțumim pentru că ajuți la menținerea securității sistemului nostru! 🔒
