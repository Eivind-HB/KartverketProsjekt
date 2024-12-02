using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kartverket.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FylkesInfo",
                columns: table => new
                {
                    FylkesNameID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FylkesName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FylkesInfo", x => x.FylkesNameID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Issues",
                columns: table => new
                {
                    issueNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    IssueType = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Issues", x => x.issueNo);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KartverketEmployee",
                columns: table => new
                {
                    EmployeeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PhoneNo = table.Column<int>(type: "int", nullable: false),
                    Mail = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Title = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Wage = table.Column<int>(type: "int", nullable: false),
                    Firstname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lastname = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KartverketEmployee", x => x.EmployeeID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "KommuneInfo",
                columns: table => new
                {
                    KommuneInfoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KommuneName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KommuneInfo", x => x.KommuneInfoID);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusNo);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CaseWorkers",
                columns: table => new
                {
                    CaseWorkerID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    KartverketEmployee_EmployeeID = table.Column<int>(type: "int", nullable: false),
                    Password = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MustChangePassword = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseWorkers", x => x.CaseWorkerID);
                    table.ForeignKey(
                        name: "FK_CaseWorkers_KartverketEmployee_KartverketEmployee_EmployeeID",
                        column: x => x.KartverketEmployee_EmployeeID,
                        principalTable: "KartverketEmployee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Mail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Password = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CaseWorkerUser = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_Users_CaseWorkers_CaseWorkerUser",
                        column: x => x.CaseWorkerUser,
                        principalTable: "CaseWorkers",
                        principalColumn: "CaseWorkerID");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Case",
                columns: table => new
                {
                    CaseNo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LocationInfo = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommentCaseWorker = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    User_UserID = table.Column<int>(type: "int", nullable: false),
                    IssueNo = table.Column<int>(type: "int", nullable: false),
                    Images = table.Column<byte[]>(type: "longblob", nullable: true),
                    KommuneNo = table.Column<int>(type: "int", nullable: false),
                    FylkesNo = table.Column<int>(type: "int", nullable: false),
                    StatusNo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Case", x => x.CaseNo);
                    table.ForeignKey(
                        name: "FK_Case_FylkesInfo_FylkesNo",
                        column: x => x.FylkesNo,
                        principalTable: "FylkesInfo",
                        principalColumn: "FylkesNameID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Case_Issues_IssueNo",
                        column: x => x.IssueNo,
                        principalTable: "Issues",
                        principalColumn: "issueNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Case_KommuneInfo_KommuneNo",
                        column: x => x.KommuneNo,
                        principalTable: "KommuneInfo",
                        principalColumn: "KommuneInfoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Case_Status_StatusNo",
                        column: x => x.StatusNo,
                        principalTable: "Status",
                        principalColumn: "StatusNo",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Case_Users_User_UserID",
                        column: x => x.User_UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CaseWorkerAssignment",
                columns: table => new
                {
                    CaseNo = table.Column<int>(type: "int", nullable: false),
                    CaseWorkerID = table.Column<int>(type: "int", nullable: false),
                    PaidHours = table.Column<decimal>(type: "decimal(65,30)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CaseWorkerAssignment", x => new { x.CaseNo, x.CaseWorkerID });
                    table.ForeignKey(
                        name: "FK_CaseWorkerAssignment_CaseWorkers_CaseWorkerID",
                        column: x => x.CaseWorkerID,
                        principalTable: "CaseWorkers",
                        principalColumn: "CaseWorkerID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CaseWorkerAssignment_Case_CaseNo",
                        column: x => x.CaseNo,
                        principalTable: "Case",
                        principalColumn: "CaseNo",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "FylkesInfo",
                columns: new[] { "FylkesNameID", "FylkesName" },
                values: new object[,]
                {
                    { 3, "Oslo" },
                    { 11, "Rogaland" },
                    { 15, "Møre og Romsdal" },
                    { 18, "Nordland - Nordlánnda" },
                    { 31, "Østfold" },
                    { 32, "Akershus" },
                    { 33, "Buskerud" },
                    { 34, "Innlandet" },
                    { 39, "Vestfold" },
                    { 40, "Telemark" },
                    { 42, "Agder" },
                    { 46, "Vestland" },
                    { 50, "Trøndelag - Trööndelage" },
                    { 55, "Troms - Romsa - Tromssa" },
                    { 56, "Finnmark - Finnmárku - Finmarkku" },
                    { 100, "Sjø uten fylke" }
                });

            migrationBuilder.InsertData(
                table: "Issues",
                columns: new[] { "issueNo", "IssueType" },
                values: new object[,]
                {
                    { 1, "Adresse/Tomt" },
                    { 2, "Vei/Sti" },
                    { 3, "Sjø" },
                    { 4, "Annet" }
                });

            migrationBuilder.InsertData(
                table: "KartverketEmployee",
                columns: new[] { "EmployeeID", "Firstname", "Lastname", "Mail", "PhoneNo", "Title", "Wage" },
                values: new object[,]
                {
                    { 1, "Admin", "Adminsen", "admin@kartverket.no", 0, "Admin", 0 },
                    { 101, "Erik", "Hansen", "erik.hansen@kartverket.no", 12345678, "SeniorSurveyor", 650000 },
                    { 102, "Maria", "Olsen", "maria.olsen@kartverket.no", 23456789, "GIS Specialist", 600000 },
                    { 103, "Anders", "Berg", "anders.berg@kartverket.no", 34567890, "Property Lawyer", 700000 },
                    { 104, "Sofia", "Larsen", "sofia.larsen@kartverket.no", 45678901, "Cartographer", 580000 },
                    { 105, "Nils", "Bakken", "nils.bakken@kartverket.no", 56789012, "Data Analyst", 620000 },
                    { 106, "Line", "Pedersen", "line.pedersen@kartverket.no", 67890123, "GIS Manager", 680000 },
                    { 107, "Jonas", "Andreassen", "jonas.andreassen@kartverket.no", 78961234, "Senior Surveyor", 660000 },
                    { 108, "Emma", "Kristiansen", "emma.kristiansen@kartverket.no", 89012345, "Cartography Specialist", 590000 },
                    { 109, "Martin", "Johansen", "martin.johansen@kartverket.no", 90123456, "Data Analyst", 610000 },
                    { 110, "Ingrid", "Nelson", "ingrid.nelson@kartverket.no", 1234567, "Project Coordinator", 630000 },
                    { 111, "Magnus", "Olsen", "magnus.olsen@kartverket.no", 12345478, "Remote SensingExpert", 670000 },
                    { 112, "Sara", "Hansen", "sara.hansen@kartverket.no", 23459789, "Legal Advisor", 690000 },
                    { 113, "Daniel", "Berg", "daniel.berg@kartverket.no", 34517890, "Technical Specialist", 640000 },
                    { 114, "Julia", "Larsen", "julia.larsen@kartverket.no", 45671901, "Research Coordinator", 620000 },
                    { 115, "Thomas", "Jensen", "thomas.jensen@kartverket.no", 56779012, "Senior Analyst", 650000 },
                    { 116, "Maria", "Andersen", "maria.andersen@kartverket.no", 67820123, "Geospatial Specialist", 600000 },
                    { 117, "Alexander", "Pedersen", "alexander.pedersen@kartverket.no", 78901234, "Field Operations Manager", 680000 },
                    { 118, "Sofia", "Olsen", "sofia.olsen@kartverket.no", 83012345, "Data Visualization Expert", 610000 },
                    { 119, "Hans", "Hansen", "hans.hansen@kartverket.no", 90323436, "Senior Consultant", 670000 },
                    { 120, "Linnea", "Berg", "linnea.berg@kartverket.no", 1234267, "Research Analyst", 590000 }
                });

            migrationBuilder.InsertData(
                table: "KommuneInfo",
                columns: new[] { "KommuneInfoID", "KommuneName" },
                values: new object[,]
                {
                    { 158, "Hara" },
                    { 301, "Oslo" },
                    { 1101, "Eigersund" },
                    { 1103, "Stavanger" },
                    { 1106, "Haugesund" },
                    { 1108, "Sandnes" },
                    { 1111, "Sokndal" },
                    { 1112, "Lund" },
                    { 1114, "Bjerkreim" },
                    { 1119, "Hå" },
                    { 1120, "Klepp" },
                    { 1121, "Time" },
                    { 1122, "Gjesdal" },
                    { 1124, "Sola" },
                    { 1127, "Randaberg" },
                    { 1130, "Strand" },
                    { 1133, "Hjelmeland" },
                    { 1134, "Suldal" },
                    { 1135, "Sauda" },
                    { 1144, "Kvitsøy" },
                    { 1145, "Bokn" },
                    { 1146, "Tysvær" },
                    { 1149, "Karmøy" },
                    { 1151, "Utsira" },
                    { 1160, "Vindafjord" },
                    { 1505, "Kristiansund" },
                    { 1506, "Molde" },
                    { 1508, "Ålesund" },
                    { 1511, "Vanylven" },
                    { 1514, "Sande" },
                    { 1515, "Herøy" },
                    { 1516, "Ulstein" },
                    { 1517, "Hareid" },
                    { 1520, "Ørsta" },
                    { 1525, "Stranda" },
                    { 1528, "Sykkylven" },
                    { 1531, "Sula" },
                    { 1532, "Giske" },
                    { 1535, "Vestnes" },
                    { 1539, "Rauma" },
                    { 1547, "Aukra" },
                    { 1554, "Averøy" },
                    { 1557, "Gjemnes" },
                    { 1560, "Tingvoll" },
                    { 1563, "Sunndal" },
                    { 1566, "Surnadal" },
                    { 1573, "Smøla" },
                    { 1576, "Aure" },
                    { 1577, "Volda" },
                    { 1578, "Fjord" },
                    { 1579, "Hustadvika" },
                    { 1804, "Bodø" },
                    { 1806, "Narvik" },
                    { 1811, "Bindal" },
                    { 1812, "Sømna" },
                    { 1813, "Brønnøy" },
                    { 1815, "Vega" },
                    { 1816, "Vevelstad" },
                    { 1818, "Herøy" },
                    { 1820, "Alstahaug" },
                    { 1822, "Leirfjord" },
                    { 1824, "Vefsn" },
                    { 1825, "Grane" },
                    { 1826, "Aarborte - Hattfjelldal" },
                    { 1827, "Dønna" },
                    { 1828, "Nesna" },
                    { 1832, "Hemnes" },
                    { 1833, "Rana" },
                    { 1834, "Lurøy" },
                    { 1835, "Træna" },
                    { 1836, "Rødøy" },
                    { 1837, "Meløy" },
                    { 1838, "Gildeskål" },
                    { 1839, "Beiarn" },
                    { 1840, "Saltdal" },
                    { 1841, "Fauske - Fuossko" },
                    { 1845, "Sørfold" },
                    { 1848, "Steigen" },
                    { 1851, "Lødingen" },
                    { 1853, "Evenes - Evenášši" },
                    { 1856, "Røst" },
                    { 1857, "Værøy" },
                    { 1859, "Flakstad" },
                    { 1860, "Vestvågøy" },
                    { 1865, "Vågan" },
                    { 1866, "Hadsel" },
                    { 1867, "Bø" },
                    { 1868, "Øksnes" },
                    { 1870, "Sortland - Suortá" },
                    { 1871, "Andøy" },
                    { 1874, "Moskenes" },
                    { 1875, "Hábmer - Hamarøy" },
                    { 3101, "Halden" },
                    { 3103, "Moss" },
                    { 3105, "Sarpsborg" },
                    { 3107, "Fredrikstad" },
                    { 3110, "Hvaler" },
                    { 3112, "Råde" },
                    { 3114, "Våler" },
                    { 3116, "Skiptvet" },
                    { 3118, "IndreØstfold" },
                    { 3120, "Rakkestad" },
                    { 3122, "Marker" },
                    { 3124, "Aremark" },
                    { 3201, "Bærum" },
                    { 3203, "Asker" },
                    { 3205, "Lillestrøm" },
                    { 3207, "NordreFollo" },
                    { 3209, "Ullensaker" },
                    { 3212, "Nesodden" },
                    { 3214, "Frogn" },
                    { 3216, "Vestby" },
                    { 3218, "Ås" },
                    { 3220, "Enebakk" },
                    { 3222, "Lørenskog" },
                    { 3224, "Rælingen" },
                    { 3226, "Aurskog - Høland" },
                    { 3228, "Nes" },
                    { 3230, "Gjerdrum" },
                    { 3232, "Nittedal" },
                    { 3234, "Lunner" },
                    { 3236, "Jevnaker" },
                    { 3238, "Nannestad" },
                    { 3240, "Eidsvoll" },
                    { 3242, "Hurdal" },
                    { 3301, "Drammen" },
                    { 3303, "Kongsberg" },
                    { 3305, "Ringerike" },
                    { 3310, "Hole" },
                    { 3312, "Lier" },
                    { 3314, "ØvreEiker" },
                    { 3316, "Modum" },
                    { 3318, "Krødsherad" },
                    { 3320, "Flå" },
                    { 3322, "Nesbyen" },
                    { 3324, "Gol" },
                    { 3326, "Hemsedal" },
                    { 3328, "Ål" },
                    { 3330, "Hol" },
                    { 3332, "Sigdal" },
                    { 3334, "Flesberg" },
                    { 3336, "Rollag" },
                    { 3338, "NoreogUvdal" },
                    { 3401, "Kongsvinger" },
                    { 3403, "Hamar" },
                    { 3405, "Lillehammer" },
                    { 3407, "Gjøvik" },
                    { 3411, "Ringsaker" },
                    { 3412, "Løten" },
                    { 3413, "Stange" },
                    { 3414, "Nord - Odal" },
                    { 3415, "Sør - Odal" },
                    { 3416, "Eidskog" },
                    { 3417, "Grue" },
                    { 3418, "Åsnes" },
                    { 3419, "Våler" },
                    { 3420, "Elverum" },
                    { 3421, "Trysil" },
                    { 3422, "Åmot" },
                    { 3423, "Stor - Elvdal" },
                    { 3424, "Rendalen" },
                    { 3425, "Engerdal" },
                    { 3426, "Tolga" },
                    { 3427, "Tynset" },
                    { 3428, "Alvdal" },
                    { 3429, "Folldal" },
                    { 3430, "Os" },
                    { 3431, "Dovre" },
                    { 3432, "Lesja" },
                    { 3433, "Skjåk" },
                    { 3434, "Lom" },
                    { 3435, "Vågå" },
                    { 3436, "Nord - Fron" },
                    { 3437, "Sel" },
                    { 3438, "Sør - Fron" },
                    { 3439, "Ringebu" },
                    { 3440, "Øyer" },
                    { 3441, "Gausdal" },
                    { 3442, "ØstreToten" },
                    { 3443, "VestreToten" },
                    { 3446, "Gran" },
                    { 3447, "SøndreLand" },
                    { 3448, "NordreLand" },
                    { 3449, "Sør - Aurdal" },
                    { 3450, "Etnedal" },
                    { 3451, "Nord - Aurdal" },
                    { 3452, "VestreSlidre" },
                    { 3453, "ØystreSlidre" },
                    { 3454, "Vang" },
                    { 3901, "Horten" },
                    { 3903, "Holmestrand" },
                    { 3905, "Tønsberg" },
                    { 3907, "Sandefjord" },
                    { 3909, "Larvik" },
                    { 3911, "Færder" },
                    { 4001, "Porsgrunn" },
                    { 4003, "Skien" },
                    { 4005, "Notodden" },
                    { 4010, "Siljan" },
                    { 4012, "Bamble" },
                    { 4014, "Kragerø" },
                    { 4016, "Drangedal" },
                    { 4018, "Nome" },
                    { 4020, "Midt - Telemark" },
                    { 4022, "Seljord" },
                    { 4024, "Hjartdal" },
                    { 4026, "Tinn" },
                    { 4028, "Kviteseid" },
                    { 4030, "Nissedal" },
                    { 4032, "Fyresdal" },
                    { 4034, "Tokke" },
                    { 4036, "Vinje" },
                    { 4201, "Risør" },
                    { 4202, "Grimstad" },
                    { 4203, "Arendal" },
                    { 4204, "Kristiansand" },
                    { 4205, "Lindesnes" },
                    { 4206, "Farsund" },
                    { 4207, "Flekkefjord" },
                    { 4211, "Gjerstad" },
                    { 4212, "Vegårshei" },
                    { 4213, "Tvedestrand" },
                    { 4214, "Froland" },
                    { 4215, "Lillesand" },
                    { 4216, "Birkenes" },
                    { 4217, "Åmli" },
                    { 4218, "Iveland" },
                    { 4219, "EvjeogHornnes" },
                    { 4220, "Bygland" },
                    { 4221, "Valle" },
                    { 4222, "Bykle" },
                    { 4223, "Vennesla" },
                    { 4224, "Åseral" },
                    { 4225, "Lyngdal" },
                    { 4226, "Hægebostad" },
                    { 4227, "Kvinesdal" },
                    { 4228, "Sirdal" },
                    { 4601, "Bergen" },
                    { 4602, "Kinn" },
                    { 4611, "Etne" },
                    { 4612, "Sveio" },
                    { 4613, "Bømlo" },
                    { 4614, "Stord" },
                    { 4615, "Fitjar" },
                    { 4616, "Tysnes" },
                    { 4617, "Kvinnherad" },
                    { 4618, "Ullensvang" },
                    { 4619, "Eidfjord" },
                    { 4620, "Ulvik" },
                    { 4621, "Voss" },
                    { 4622, "Kvam" },
                    { 4623, "Samnanger" },
                    { 4624, "Bjørnafjorden" },
                    { 4625, "Austevoll" },
                    { 4626, "Øygarden" },
                    { 4627, "Askøy" },
                    { 4628, "Vaksdal" },
                    { 4629, "Modalen" },
                    { 4630, "Osterøy" },
                    { 4631, "Alver" },
                    { 4632, "Austrheim" },
                    { 4633, "Fedje" },
                    { 4634, "Masfjorden" },
                    { 4635, "Gulen" },
                    { 4636, "Solund" },
                    { 4637, "Hyllestad" },
                    { 4638, "Høyanger" },
                    { 4639, "Vik" },
                    { 4640, "Sogndal" },
                    { 4641, "Aurland" },
                    { 4642, "Lærdal" },
                    { 4643, "Årdal" },
                    { 4644, "Luster" },
                    { 4645, "Askvoll" },
                    { 4646, "Fjaler" },
                    { 4647, "Sunnfjord" },
                    { 4648, "Bremanger" },
                    { 4649, "Stad" },
                    { 4650, "Gloppen" },
                    { 4651, "Stryn" },
                    { 5001, "Trondheim - Tråante" },
                    { 5006, "Steinkjer" },
                    { 5007, "Namsos - Nåavmesjenjaelmie" },
                    { 5014, "Frøya" },
                    { 5020, "Osen" },
                    { 5021, "Oppdal" },
                    { 5022, "Rennebu" },
                    { 5025, "Rosse - Røros" },
                    { 5026, "Holtålen" },
                    { 5027, "MidtreGauldal" },
                    { 5028, "Melhus" },
                    { 5029, "Skaun" },
                    { 5031, "Malvik" },
                    { 5032, "Selbu" },
                    { 5033, "Tydal" },
                    { 5034, "Meråker" },
                    { 5035, "Stjørdal" },
                    { 5036, "Frosta" },
                    { 5037, "Levanger" },
                    { 5038, "Verdal" },
                    { 5041, "Snåase - Snåsa" },
                    { 5042, "Lierne" },
                    { 5043, "Raarvihke - Røyrvik" },
                    { 5044, "Namsskogan" },
                    { 5045, "Grong" },
                    { 5046, "Høylandet" },
                    { 5047, "Overhalla" },
                    { 5049, "Flatanger" },
                    { 5052, "Leka" },
                    { 5053, "Inderøy" },
                    { 5054, "IndreFosen" },
                    { 5055, "Heim" },
                    { 5056, "Hitra" },
                    { 5057, "Ørland" },
                    { 5058, "Åfjord" },
                    { 5059, "Orkland" },
                    { 5060, "Nærøysund" },
                    { 5061, "Rindal" },
                    { 5501, "Tromsø" },
                    { 5503, "Harstad - Hárstták" },
                    { 5510, "Kvæfjord" },
                    { 5512, "Tjeldsund - Dielddanuorri" },
                    { 5514, "Ibestad" },
                    { 5516, "Gratangen" },
                    { 5518, "Loabák - Lavangen" },
                    { 5520, "Bardu" },
                    { 5522, "Salangen" },
                    { 5524, "Målselv" },
                    { 5526, "Sørreisa" },
                    { 5528, "Dyrøy" },
                    { 5530, "Senja" },
                    { 5532, "Balsfjord" },
                    { 5534, "Karlsøy" },
                    { 5536, "Lyngen" },
                    { 5538, "Storfjord - Omasvuotna - Omasvuono" },
                    { 5540, "Gáivuotna - Kåfjord - Kaivuono" },
                    { 5542, "Skjervøy" },
                    { 5544, "Nordreisa - Ráisa - Raisi" },
                    { 5546, "Kvænangen" },
                    { 5601, "Alta" },
                    { 5603, "Hammerfest - Hámmerfeasta" },
                    { 5605, "Sør - Varanger" },
                    { 5607, "Vadsø" },
                    { 5610, "Kárášjohka - Karasjok" },
                    { 5612, "Guovdageaidnu - Kautokeino" },
                    { 5614, "Loppa" },
                    { 5616, "Hasvik" },
                    { 5618, "Måsøy" },
                    { 5620, "Nordkapp" },
                    { 5622, "Porsanger - Porsáŋgu - Porsanki" },
                    { 5624, "Lebesby" },
                    { 5626, "Gamvik" },
                    { 5628, "Deatnu - Tana" },
                    { 5630, "Berlevåg" },
                    { 5632, "Båtsfjord" },
                    { 5634, "Vardø" },
                    { 5636, "Unjárga - Nesseby" },
                    { 100100, "Sjø uten kommune" }
                });

            migrationBuilder.InsertData(
                table: "Status",
                columns: new[] { "StatusNo", "StatusName" },
                values: new object[,]
                {
                    { 1, "Sendt" },
                    { 2, "Mottat" },
                    { 3, "Behandles" },
                    { 4, "Fullført" },
                    { 5, "Avvist" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "CaseWorkerUser", "Mail", "Password", "UserName" },
                values: new object[] { 404, null, "NoUser@kartverket.no", "password", "NoUserProfile" });

            migrationBuilder.InsertData(
                table: "CaseWorkers",
                columns: new[] { "CaseWorkerID", "KartverketEmployee_EmployeeID", "MustChangePassword", "Password" },
                values: new object[,]
                {
                    { 1, 1, false, "default" },
                    { 201, 101, false, "default" },
                    { 202, 102, false, "default" },
                    { 203, 103, false, "default" },
                    { 204, 104, false, "default" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Case_FylkesNo",
                table: "Case",
                column: "FylkesNo");

            migrationBuilder.CreateIndex(
                name: "IX_Case_IssueNo",
                table: "Case",
                column: "IssueNo");

            migrationBuilder.CreateIndex(
                name: "IX_Case_KommuneNo",
                table: "Case",
                column: "KommuneNo");

            migrationBuilder.CreateIndex(
                name: "IX_Case_StatusNo",
                table: "Case",
                column: "StatusNo");

            migrationBuilder.CreateIndex(
                name: "IX_Case_User_UserID",
                table: "Case",
                column: "User_UserID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseWorkerAssignment_CaseWorkerID",
                table: "CaseWorkerAssignment",
                column: "CaseWorkerID");

            migrationBuilder.CreateIndex(
                name: "IX_CaseWorkers_KartverketEmployee_EmployeeID",
                table: "CaseWorkers",
                column: "KartverketEmployee_EmployeeID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CaseWorkerUser",
                table: "Users",
                column: "CaseWorkerUser",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CaseWorkerAssignment");

            migrationBuilder.DropTable(
                name: "Case");

            migrationBuilder.DropTable(
                name: "FylkesInfo");

            migrationBuilder.DropTable(
                name: "Issues");

            migrationBuilder.DropTable(
                name: "KommuneInfo");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "CaseWorkers");

            migrationBuilder.DropTable(
                name: "KartverketEmployee");
        }
    }
}
