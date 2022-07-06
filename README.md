### .env structure

    # Shared
    POSTGRESUSER=root
    POSTGRESPASSWORD=root
    
    # Cards api shared
    CRUDCARDSAPIOUTPUTPORT=[SET PORT]
    
    # Cards Dapper connection string
    CARDSDATABASEEFCOREHOST=db
    CARDSDATABASEDAPPERNAME=DebetCardsDapper
    
    # Cards EF connection string
    CARDSDATABASEEFCOREHOST=db
    CARDSDATABASEEFCORENAME=DebetCardsEF
    
    # Identity connection string
    POSTGRESIDENTITYEFCORE=Host=identitydb;Database=IdentityDB;User Id=root;Password=root
    IDENTITYSECURECODE=Some Creapy Code
    IDENTITYSERVEROUTPUTPORT=[SET PORT]
    
    # Postgres Identity api configuration
    POSTGRESIDENTITYOUTPORT=[SET PORT]
    
    # Postgrees Cards api configuration
    POSTGRESOUTPORT=[SET PORT]


> Don't forget to set ports in marks [SET PORT]