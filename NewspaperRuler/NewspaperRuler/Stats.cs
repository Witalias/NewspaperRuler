﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Text.Json;
using System.Linq;

namespace NewspaperRuler
{
    public partial class Stats : ICloneable
    {
        public static string MonetaryCurrencyName { get; } = "ТОКЕНОВ";
        private readonly GraphicObject noteBackground;
        private readonly DayEnd dayEnd;

        private int degreeGovernmentAnger = 0;
        private int rentDebts = 0;
        private int productsDebts = 0;
        private int heatingDebts = 0;

        private int rent = 120;
        private int productsCost = 40;
        private int heatingCost = 30;

        private int money;
        private int loyalityFactor = 1;
        private Difficulties difficulty;

        public DateTime Date { get; private set; } = new DateTime(1987, 9, 26);
        public bool DecreesAreVisible { get; private set; } = false;
        public bool TrendsAreVisible { get; private set; } = false;

        public int Money
        {
            get => money;
            set
            {
                if (value < 0) money = 0;
                else money = value;
            }
        }

        public int LevelNumber { get; private set; } = 1;

        private int loyality = 0;
        private int requiredLoyality;

        private int ministrySatisfactionsCount = 0;


        public LevelData Level { get; set; }

        private List<Dictionary<string, bool>> flags;

        public Stats(DayEnd dayEnd)
        {
            this.dayEnd = dayEnd;
            noteBackground = new GraphicObject(Properties.Resources.NoteBackground, 750, 1000, 125);
            flags = new List<Dictionary<string, bool>>
            {
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleAboutChampionWasApproved"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["ArticleOnProhibitionWeaponsWasApproved"] = false,
                    ["MainCharacterWasOnDate"] = false,
                    ["ArticleOnMassStarvationWasApproved"] = false,
                    ["ArticleOnDryRationsWasApproved"] = false,
                    ["ArticleAboutSalaryDelayWasApproved"] = false,
                    ["SalaryIncreased"] = false

                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterWasOnDate"] = false,
                    ["ArticleOnProhibitionWeaponsWasApproved"] = false,
                    ["TimasStudiesWerePaid"] = true
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["TheMainCharacterPaidForSilence"] = true,
                    ["TheMainCharacterPaidLarisa"] = true,
                    ["MainCharacterGaveOutAboutSecretEditorialOffice"] = false,
                    ["MissingPersonNoticeWasPublished"] = false,
                    ["AnnouncementOfDisappearanceOfGalinasHusbandWasApproved"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterWentToFestival"] = true,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["WifeIsFaithful"] = false,
                    ["MainCharacterHelpedGrasshoppers"] = false,
                    ["MainCharacterBoughtPresentForHisSon"] = true,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterHelpedGrasshoppersFirstTime"] = false,
                    ["MainCharacterHelpedGrasshoppersSecondTime"] = false,
                    ["GalinaWillHelpMainCharacterFreeCharge"] = false,
                    ["MainCharacterPaidGalina"] = true,
                    ["MedicineWasDeliveredToWife"] = true,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["MainCharacterHelpedGrasshoppersFirstTime"] = false,
                    ["MainCharacterHelpedGrasshoppersSecondTime"] = false,
                    ["MainCharacterHelpedGrasshoppersThirdTime"] = false,
                    ["SonStayedAtHome"] = false,
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["GrasshoppersEliminated"] = false,
                    ["TimasStudiesWerePaid"] = true
                },
                new Dictionary<string, bool>
                {
                    ["MinistryIsSatisfied"] = false,
                    ["WifesOperationPaid"] = true,
                    ["MainCharacterBoughtFakePassports"] = true,
                    ["MainCharacterPlantedBomb"] = false,
                    ["WifeAlive"] = false,
                    ["SonAlive"] = false,
                    ["MainCharacterIsFree"] = false,
                }
            };
        }

        public void IncreaseLevelNumber() => ++LevelNumber;

        public (string, Bitmap)[] GetGameResults()
        {
            var result = new List<(string, Bitmap)>();

            if ((flags[6]["MedicineWasDeliveredToWife"] || flags[9]["WifesOperationPaid"]) && flags[7]["SonStayedAtHome"])
            {
                result.Add(("Жена выздоровела. Сын избежал призыва на войну. Все живы.", new Bitmap(Properties.Resources.WifeAliveSonAlive, Scale.Get(500), Scale.Get(500))));
                flags[9]["WifeAlive"] = true;
                flags[9]["SonAlive"] = true;
            }
            else if ((flags[6]["MedicineWasDeliveredToWife"] || flags[9]["WifesOperationPaid"]) && !flags[7]["SonStayedAtHome"])
            {
                result.Add(("Жена выздоровела. Сына забрали на войну. Он не вернулся...", new Bitmap(Properties.Resources.WifeAliveSonDead, Scale.Get(500), Scale.Get(500))));
                if (!flags[5]["WifeIsFaithful"])
                    result.Add(("Однако жена не могла смириться с Вашим предательством и смертью сына. Она свела свою жизнь с концами...", new Bitmap(Properties.Resources.Gallows, Scale.Get(500), Scale.Get(500))));
                else flags[9]["WifeAlive"] = true;
            }
            else if (!(flags[6]["MedicineWasDeliveredToWife"] || flags[9]["WifesOperationPaid"]) && flags[7]["SonStayedAtHome"])
            {
                flags[9]["SonAlive"] = true;
                result.Add(("Жена умерла. Сын избежал призыва на войну.", new Bitmap(Properties.Resources.WifeDeadSonAlive, Scale.Get(500), Scale.Get(500))));
            }
            else result.Add(("Жена умерла. Сына забрали на войну. Он не вернулся...", new Bitmap(Properties.Resources.WifeDeadSonDead, Scale.Get(500), Scale.Get(500))));

            if (!flags[9]["MainCharacterPlantedBomb"])
            {
                if (flags[9]["SonAlive"] || flags[9]["WifeAlive"])
                {
                    result.Add(("Бомбу оперативно обезвредили сапёры. Узнав об опасности, которая нависла над Вашей семьёй, Вы обратились в Министерство правопорядка.", new Bitmap(Properties.Resources.Bomb, Scale.Get(500), Scale.Get(500))));
                    if (ministrySatisfactionsCount <= 8)
                    {
                        flags[9]["SonAlive"] = false;
                        flags[9]["WifeAlive"] = false;
                        result.Add(("Но там не восприняли Вас всерьёз, указав на Вашу беззалаберность и неисполнение приказов при служении государству. Дом, в котором жила Ваша семья, ночью был заминирован и взорван. Все его жильцы трагически погибли.", new Bitmap(Properties.Resources.House, Scale.Get(500), Scale.Get(400))));
                    }
                    else
                        result.Add(("Вы отлично служили государству и совершали минимум ошибок, поэтому Вам готовы помочь! Стражи правопорядка вовремя прибыли к дому, где жила Ваша семья, и предотвратили минирование. Преступная организация поймана, жильцы дома остались живы.", new Bitmap(Properties.Resources.NoBomb, Scale.Get(500), Scale.Get(500))));
                }
                else
                    result.Add(("Бомбу оперативно обезвредили сапёры. А дом, в котором жили покойные члены семьи, был заминирован и взорван. Все жильцы трагически погибли.", new Bitmap(Properties.Resources.Bomb, Scale.Get(500), Scale.Get(500))));
            }

            if (!flags[8]["GrasshoppersEliminated"])
                result.Add(("\"Кузнечики\" осуществили попытку захватить гос. власть, однако государство оказалось сильнее...", new Bitmap(Properties.Resources.Grasshopper, Scale.Get(500), Scale.Get(500))));

            if (flags[9]["MainCharacterBoughtFakePassports"])
            {
                flags[9]["MainCharacterIsFree"] = true;

                var bitmap = new Bitmap(Properties.Resources.Ubringston, Scale.Get(500), Scale.Get(400));
                if (flags[9]["SonAlive"] && flags[9]["WifeAlive"])
                    result.Add(("Вы с семьёй успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else if (flags[9]["SonAlive"])
                    result.Add(("Вы с сыном успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else if (flags[9]["WifeAlive"])
                    result.Add(("Вы с женой успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));
                else result.Add(("Вы в одиночку успешно сбежали в Убрингстон, где начали жизнь с чистого листа.", bitmap));

                if (flags[6]["GalinaWillHelpMainCharacterFreeCharge"] || flags[6]["MainCharacterPaidGalina"])
                    result.Add(("В Убрингстоне Вы вновь встретились с Галиной Руш. Она стала Вашим частым гостем.", new Bitmap(Properties.Resources.Galina, Scale.Get(450), Scale.Get(500))));
            }
            else
            {
                result.Add(("Вы прибыли на слушание по делу причастия гос. служащих к \"Кузнечикам\".", new Bitmap(Properties.Resources.Hummer, Scale.Get(500), Scale.Get(500))));
                if (!flags[8]["GrasshoppersEliminated"])
                {
                    result.Add(("Против Вас найдены улики в Вашем причастии к \"Кузнечикам\". За измену государству Вам положено наказание в виде лишения свободы на 10 лет.", new Bitmap(Properties.Resources.Prison, Scale.Get(600), Scale.Get(450))));
                    if (!flags[3]["MainCharacterGaveOutAboutSecretEditorialOffice"])
                    {
                        result.Add(("Разъярённая толпа граждан из тайной редакции, существование которой Вы оставили в секрете, силой добилась Вашего освобождения.", new Bitmap(Properties.Resources.Crowd, Scale.Get(600), Scale.Get(400))));
                        flags[9]["MainCharacterIsFree"] = true;
                    }
                }
                else
                {
                    flags[9]["MainCharacterIsFree"] = true;
                    result.Add(("Вы абсолютно чисты, и с Вас сняты все подозрения. Государство благодарит Вас за преданность!", new Bitmap(Properties.Resources.Hummer, Scale.Get(500), Scale.Get(500))));
                }
            }

            if (flags[9]["MainCharacterPlantedBomb"])
            {
                result.Add(("Здание Министерства цензуры и печати было взорвано по Вашей инициативе. Все находящиеся в нём гос. служащие трагически погибли. В ходе расследования стражи правопорядка вышли на Ваш след.", new Bitmap(Properties.Resources.Bomb, Scale.Get(500), Scale.Get(500))));
                if (flags[9]["MainCharacterBoughtFakePassports"])
                    result.Add(("Однако Вы успели скрыться за границей, и Вам ничего не грозит. Теперь Вы официально признаны предателем Родины.", new Bitmap(Properties.Resources.Ubringston, Scale.Get(500), Scale.Get(400))));
                else if (flags[9]["MainCharacterIsFree"])
                {
                    flags[9]["MainCharacterIsFree"] = false;
                    result.Add(("За столь тяжёлое преступление Вам положено наказание в виде лишения свободы на 10 лет.", new Bitmap(Properties.Resources.Prison, Scale.Get(600), Scale.Get(450))));
                }
                else
                    result.Add(("За двойное преступление Вы приговорены к смертной казни. Последнее, о чём Вы желали перед неизбежным, - встретить и обнять свою семью на небесах.", new Bitmap(Properties.Resources.Gallows, Scale.Get(500), Scale.Get(500))));
            }

            if (flags[9]["MainCharacterIsFree"])
            {
                if (flags[9]["SonAlive"])
                {
                    if (flags[5]["MainCharacterBoughtPresentForHisSon"])
                        result.Add(("Вы нашли с сыном общий язык. Теперь Вы проводите больше времени вместе.", null));
                    else result.Add(("Вы пытаетесь найти с сыном общий язык, но он сторонится Вас. Что не так?", null));
                }

                if (flags[9]["WifeAlive"])
                {
                    if (flags[5]["WifeIsFaithful"])
                        result.Add(("У Вас с женой тёплые отношения. Вы живёте в верности и согласии.", new Bitmap(Properties.Resources.Hands, Scale.Get(300), Scale.Get(500))));
                    else result.Add(("Жена отвергает Вас. Она проводит большую часть своего времени с сыном.", new Bitmap(Properties.Resources.BrokenHeart, Scale.Get(500), Scale.Get(500))));
                }
            }

            if (loyality >= requiredLoyality)
            {
                if (!flags[9]["MainCharacterBoughtFakePassports"])
                {
                    var bitmap = new Bitmap(Properties.Resources.MuchMoney, Scale.Get(500), Scale.Get(250));
                    if (flags[9]["MainCharacterIsFree"])
                    {
                        if (flags[9]["SonAlive"] && flags[9]["WifeAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам и Вашей семье выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. О таких богатствах Вы и не могли мечтать!", bitmap));
                        else if (flags[9]["SonAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам и Вашему сыну выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но Вас не радовали эти богатства. Вы хотели лишь одного: вернуть любимую.", bitmap));
                        else if (flags[9]["WifeAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам и Вашей жене выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но вас не радовали эти богатства. Вы с женой хотели лишь одного: вернуть сына.", bitmap));
                        else
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но вас не радовали эти богатства. Вы хотели лишь одного: вернуть свою семью.", bitmap));
                    }
                    else
                    {
                        if (flags[9]["SonAlive"] && flags[9]["WifeAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вашей семье выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но их не радовали эти богатства. Они хотели лишь одного: вернуть Вас.", bitmap));
                        else if (flags[9]["SonAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вашему сыну выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но его не радовали эти богатства. Он хотел лишь одного: вернуть своих родителей.", bitmap));
                        else if (flags[9]["WifeAlive"])
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вашей жене выделена квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ. Но её не радовали эти богатства. Она хотела лишь одного: вернуть свою семью.", bitmap));
                        else
                            result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам полагалась квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ, однако никому из членов семьи не суждено увидеть эти богатства.", bitmap));
                    }
                }
                else
                    result.Add(("Вы смогли добиться высокой лояльности граждан к государству. Вам полагалась квартира премиум-класса в центре столицы и 50000 ТОКЕНОВ, однако спокойная, мирная, тихая жизнь за границей не стоит таких богатств.", new Bitmap(Properties.Resources.NoMuchMoney, Scale.Get(500), Scale.Get(250))));
            }
            else
                result.Add(("Вы не смогли добиться высокой лояльности граждан к государству. Страна погрязла в народных волнениях и протестах.", new Bitmap(Properties.Resources.Protest, Scale.Get(500), Scale.Get(300))));

            if (flags[9]["SonAlive"])
            {
                if (flags[2]["TimasStudiesWerePaid"] && flags[8]["TimasStudiesWerePaid"])
                    result.Add(("Сын достигает отличных успехов в высшей академии. Там он нашёл свою вторую половинку. Теперь они счастливы вместе.", new Bitmap(Properties.Resources.Confederate, Scale.Get(500), Scale.Get(400))));
                else
                {
                    result.Add(("Сын устроился в бригаду лесозаготовительного завода. Он несчастен, потому что не поступил в высшую академию.", new Bitmap(Properties.Resources.NoConfederate, Scale.Get(500), Scale.Get(450))));
                    result.Add(("Но затем в стрессе он уволился и со временем спился. Он всех отвергает и не хочет никого видеть.", new Bitmap(Properties.Resources.Alcohol, Scale.Get(500), Scale.Get(500))));
                }
            }

            if (flags[9]["MainCharacterIsFree"])
            {
                if (flags[1]["ArticleOnProhibitionWeaponsWasApproved"])
                    result.Add(("Всё это время МАМБА за Вами наблюдала. Вам предложили там вакансию. " +
                        "Вы успешно прошли испытательный срок и теперь являетесь частью команды одной из самых авторитетных организаций в мире.", new Bitmap(Properties.Resources.Team, Scale.Get(600), Scale.Get(400))));
                else if (!flags[9]["MainCharacterBoughtFakePassports"])
                {
                    if (flags[4]["MainCharacterWentToFestival"])
                        result.Add(("Вы устроились на работу технического специалиста в сфере коммунального хозяйства. " +
                            "Сделать это помог Ваш новый приятель, с которым Вы познакомились на фестивале света.", new Bitmap(Properties.Resources.CommunalService, Scale.Get(500), Scale.Get(500))));
                    else result.Add(("Вы не смогли найти новую работу. Вам суждено жить в бедности до конца жизни.", new Bitmap(Properties.Resources.Trash, Scale.Get(500), Scale.Get(500))));
                }
            }

            if (flags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                result.Add(("Международная ассоциация мировой безопасности \"Апостол\" изгнала захватчиков из страны, " +
                    "передав часть Андиплантийской территории государству. Да прибудет справедливость!", new Bitmap(Properties.Resources.Town, Scale.Get(500), Scale.Get(350))));
            else
                result.Add(("Международная ассоциация мировой безопасности \"Апостол\" отказалась изгонять захватчиков из страны. " +
                    "Государство пало. Теперь эти территории присоединены к Андиплантийской коалиции.", new Bitmap(Properties.Resources.DeadTown, Scale.Get(500), Scale.Get(350))));

            result.Add(("Ничего в этой жизни не даётся легко. Чтобы достичь своих целей, необходимо идти на определенные жертвы — " +
                "тратить свои силы, время, ограничивать себя в чём-либо.", new Bitmap(Properties.Resources.Landscape, Scale.Get(500), Scale.Get(400))));
            result.Add(("Иногда бывают моменты, когда хочется всё бросить и отказаться от мечты.", new Bitmap(Properties.Resources.Fox, Scale.Get(500), Scale.Get(300))));
            result.Add(("В такие моменты вспомни, как много ты получишь, если пойдёшь дальше, и как много потеряешь, если сдашься. " +
                "Цена успеха, как правило, меньше, чем цена неудачи.", new Bitmap(Properties.Resources.Bird, Scale.Get(400), Scale.Get(500))));
            result.Add(("Единственный способ жить — это жить. Говорить себе: \"Я могу это сделать\", — даже зная, что не можешь.", new Bitmap(Properties.Resources.Flower, Scale.Get(300), Scale.Get(500))));
            result.Add(("Спасибо за игру", null));
            return result.ToArray();
        }

        public void SetFlagToTrue(string flag)
        {
            if (!flags[LevelNumber - 1].ContainsKey(flag))
                throw new Exception($"The \"{flag}\" flag doesn't exist in the collection {flags[LevelNumber - 1]}");
            flags[LevelNumber - 1][flag] = true;
        }

        public void GoToNextLevel()
        {
            if (LevelNumber > 1 && LevelNumber <= 10)
                SaveToJson();
            Date = Date.AddDays(1);
            Level = new LevelData(CreateNotes(), ArticleConstructor.ArticlesByLevel[LevelNumber - 1], loyalityFactor);
            CreateIntroduction();
            UpdateElements();
            Level.BuildEventQueue();
        }

        public void FinishLevel()
        {
            Money += Level.Salary - Level.GetTotalFine();
            LevelNumber++;

            CreateWarnings();

            dayEnd.StatsTexts.Add(GetLabel($"Лояльность граждан:\t\t{loyality}"));
            dayEnd.StatsTexts.Add(GetLabel($"Зарплата:\t\t{Level.Salary}"));
            if (Level.CurrentFine != 0) dayEnd.StatsTexts.Add(GetLabel($"Штраф:\t\t-{Level.GetTotalFine()}"));

            switch (LevelNumber - 1)
            {
                case 1:
                    dayEnd.InformationTexts.Add(GetLabel("Отметьте расходы, которые можете себе позволить."));
                    break;
                case 2:
                    rent += 30;
                    dayEnd.InformationTexts.Add(GetLabel("Квартплата увеличена."));
                    if (!flags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Подарок от Галины Руш:\t\t50"));
                    }
                    if (flags[1]["MainCharacterWasOnDate"])
                    {
                        Money -= 30;
                        dayEnd.StatsTexts.Add(GetLabel($"Посещение бара \"Алый цветок\":\t\t-30"));
                    }
                    break;
                case 3:
                    if (flags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money -= 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Штраф от Министерства социальной защиты:\t\t-50"));
                        dayEnd.InformationTexts.Add(GetLabel("Министерство социальной защиты налагает штраф за нарушение"));
                        dayEnd.InformationTexts.Add(GetLabel("неприкосновенности частной жизни гражданки Галины Руш."));
                    }
                    if (flags[1]["SalaryIncreased"])
                    {
                        Money += 100;
                        dayEnd.StatsTexts.Add(GetLabel($"Бонус к зарплате:\t\t100"));
                        dayEnd.InformationTexts.Add(GetLabel("Ваша сегодняшняя зарплата увеличена за хорошую работу."));
                    }
                    dayEnd.Expenses.Add(new Expense($"Высшая академия:\t\t100 {MonetaryCurrencyName}", 100, ExpenseType.FirstStudies));
                    break;
                case 4:
                    productsCost += 10;
                    dayEnd.InformationTexts.Add(GetLabel("Цены на продукты повысились."));
                    if (flags[1]["MainCharacterWasOnDate"] || flags[2]["MainCharacterWasOnDate"])
                        dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                    if (flags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                    {
                        Money -= 75;
                        dayEnd.StatsTexts.Add(GetLabel($"Вычет из зарплаты:\t\t-75"));
                        if (flags[1]["MainCharacterWasOnDate"] || flags[2]["MainCharacterWasOnDate"])
                            dayEnd.Expenses.Add(new Expense($"План Ларисы:\t\t25 {MonetaryCurrencyName}", 25, ExpenseType.Larisa));
                    }
                    break;
                case 5:
                    dayEnd.InformationTexts.Add(GetLabel("Становится холодно. Пора платить за отопление."));
                    dayEnd.InformationTexts.Add(GetLabel("Вы можете пойти на фестиваль света, оплатив стоимость билета."));
                    dayEnd.InformationTexts.Add(GetLabel("У Вашего сына завтра день рождения."));
                    dayEnd.Expenses.Add(new Expense($"Фестиваль света:\t\t30 {MonetaryCurrencyName}", 30, ExpenseType.Festival));
                    if (flags[1]["MainCharacterWasOnDate"] || flags[2]["MainCharacterWasOnDate"])
                    {
                        if (flags[3]["TheMainCharacterPaidLarisa"] && flags[3]["TheMainCharacterPaidForSilence"])
                        {
                            Money += 200;
                            dayEnd.StatsTexts.Add(GetLabel($"Возврат похищенных денег:\t\t200"));
                        }
                        if (!flags[3]["TheMainCharacterPaidForSilence"] && !flags[3]["TheMainCharacterPaidLarisa"])
                        {
                            flags[3]["TheMainCharacterPaidForSilence"] = true;
                            dayEnd.InformationTexts.Add(GetLabel("Сегодня последний шанс подкупить шантажистку."));
                            dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                        }
                        else if (flags[3]["TheMainCharacterPaidForSilence"] && !flags[3]["TheMainCharacterPaidLarisa"])
                            dayEnd.InformationTexts.Add(GetLabel("Шантажистка получила Ваши деньги."));
                    }
                    break;
                case 6:
                    dayEnd.InformationTexts.Add(GetLabel("Ваша жена заразилась вирусом КРАБ."));
                    dayEnd.InformationTexts.Add(GetLabel("Сегодня день рождения Вашего сына. Вы можете отправить ему подарок."));
                    dayEnd.Expenses.Add(new Expense($"Подарок сыну:\t\t35 {MonetaryCurrencyName}", 35, ExpenseType.Son));
                    if (flags[3]["MainCharacterGaveOutAboutSecretEditorialOffice"])
                    {
                        Money += 120;
                        dayEnd.StatsTexts.Add(GetLabel($"Премия:\t\t120"));
                        dayEnd.InformationTexts.Add(GetLabel("Вы получили премию за раскрытие незаконной тайной редакции."));
                    }
                    if (flags[3]["TheMainCharacterPaidForSilence"] && !flags[3]["TheMainCharacterPaidLarisa"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Шантажистка:\t\t50"));
                        dayEnd.InformationTexts.Add(GetLabel("Шантажистка решила вернуть часть похищенных денег."));
                    }
                    if (flags[5]["MainCharacterHelpedGrasshoppers"])
                    {
                        Money += 100;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t100"));
                    }
                    break;
                case 7:
                    rent += 20;
                    dayEnd.InformationTexts.Add(GetLabel("Квартплата увеличена."));
                    dayEnd.InformationTexts.Add(GetLabel("Курс лечения Вашей жены начался. Регулярно поставляйте ей лекарство. Осталось 3 дня."));
                    if ((!flags[0]["ArticleAboutChampionWasApproved"] || flags[3]["AnnouncementOfDisappearanceOfGalinasHusbandWasApproved"])
                        && !flags[6]["GalinaWillHelpMainCharacterFreeCharge"])
                        dayEnd.Expenses.Add(new Expense($"Помощь Галины Руш:\t\t110 {MonetaryCurrencyName}", 110, ExpenseType.Galina));
                    if (flags[3]["MissingPersonNoticeWasPublished"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Женщина из плиувильской аптеки украла препарат для Вашей жены и поплатилась за это:"));
                        dayEnd.InformationTexts.Add(GetLabel("её уволили. Она больше не сможет Вам помочь. Лекарство успешно доставлено Вашей жене."));
                    }
                    else dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t145 {MonetaryCurrencyName}", 145, ExpenseType.Medicine));
                    if (flags[5]["MainCharacterBoughtPresentForHisSon"])
                        dayEnd.InformationTexts.Add(GetLabel("Вашему сыну понравился подарок."));
                    else dayEnd.InformationTexts.Add(GetLabel("Ваш сын расстроился, что Вы не проявили к нему внимание в его День Рождения."));
                    if (flags[6]["MainCharacterHelpedGrasshoppersFirstTime"] && flags[6]["MainCharacterHelpedGrasshoppersSecondTime"])
                    {
                        Money += 150;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t150"));
                    }
                    break;
                case 8:
                    if (flags[6]["MedicineWasDeliveredToWife"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("До конца курса лечения Вашей жены осталось 2 дня."));
                        dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t145 {MonetaryCurrencyName}", 145, ExpenseType.Medicine));
                    }
                    if (flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                        && flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                        && flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        Money += 250;
                        dayEnd.StatsTexts.Add(GetLabel($"Привет от \"Кузнечиков\":\t\t250"));
                    }
                    else if (!flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                        && !flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                        && !flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        Money += 300;
                        dayEnd.InformationTexts.Add(GetLabel("Министерство цензуры и печати благодарит Вас за противодействие \"Кузнечикам\"."));
                        dayEnd.StatsTexts.Add(GetLabel($"Бонус к зарплате:\t\t300"));
                    }
                    break;
                case 9:
                    if (flags[6]["MedicineWasDeliveredToWife"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Сегодня последний день курса лечения Вашей жены."));
                        dayEnd.Expenses.Add(new Expense($"Лекарство для жены:\t\t145 {MonetaryCurrencyName}", 145, ExpenseType.Medicine));
                    }
                    if (!flags[6]["GalinaWillHelpMainCharacterFreeCharge"] && !flags[6]["MainCharacterPaidGalina"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Вашего сына забрали на войну."));
                        if (flags[2]["TimasStudiesWerePaid"])
                        {
                            dayEnd.InformationTexts.Add(GetLabel("Высшая академия вернула средства за обучение Вашего сына."));
                            Money += 100;
                            dayEnd.StatsTexts.Add(GetLabel($"Возврат за обучение:\t\t100"));
                        }
                    }
                    else if (flags[2]["TimasStudiesWerePaid"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Для продолжения обучения сына в высшей академии требуется орг. взнос."));
                        dayEnd.Expenses.Add(new Expense($"Высшая академия:\t\t125 {MonetaryCurrencyName}", 125, ExpenseType.SecondStudies));
                    }
                    break;
                case 10:
                    dayEnd.InformationTexts.Add(GetLabel("Вы в шаге от конца игры."));
                    if (flags[6]["MedicineWasDeliveredToWife"])
                        dayEnd.InformationTexts.Add(GetLabel("Ваша жена пошла на поправку."));
                    else
                    {
                        dayEnd.InformationTexts.Add(GetLabel("Вашу жену положили в больницу. Она умирает."));
                        dayEnd.InformationTexts.Add(GetLabel("Требуется срочная хирургическая операция – последняя надежда на её спасение."));
                        dayEnd.Expenses.Add(new Expense($"Операция:\t\t500 {MonetaryCurrencyName}", 500, ExpenseType.Operation));
                    }
                    if (flags[6]["MainCharacterHelpedGrasshoppersFirstTime"]
                                && flags[6]["MainCharacterHelpedGrasshoppersSecondTime"]
                                && flags[7]["MainCharacterHelpedGrasshoppersFirstTime"]
                                && flags[7]["MainCharacterHelpedGrasshoppersSecondTime"]
                                && flags[7]["MainCharacterHelpedGrasshoppersThirdTime"])
                    {
                        dayEnd.InformationTexts.Add(GetLabel("\"Кузнечики\" готовы достать поддельные паспорта за \"относительно низкую цену\"."));
                        dayEnd.Expenses.Add(new Expense($"Поддельные паспорта:\t\t400 {MonetaryCurrencyName}", 400, ExpenseType.Passports));
                    }
                    break;
            }
            dayEnd.StatsTexts.Add(GetLabel($"Итого:\t\t{Money} {MonetaryCurrencyName}"));

            dayEnd.Expenses.Add(new Expense($"Квартплата:\t\t{rent} {MonetaryCurrencyName}", rent, ExpenseType.Rent));
            dayEnd.Expenses.Add(new Expense($"Продукты:\t\t{productsCost} {MonetaryCurrencyName}", productsCost, ExpenseType.Products));
            if (LevelNumber > 5)
                dayEnd.Expenses.Add(new Expense($"Отопление:\t\t{heatingCost} {MonetaryCurrencyName}", heatingCost, ExpenseType.Heating));

            dayEnd.RecalculatePositions();

            Label GetLabel(string text)
            {
                return new Label
                {
                    Text = text,
                    ForeColor = Color.White,
                    Font = StringStyle.TitleFont,
                    AutoSize = true,
                };
            }

            void CreateWarnings()
            {
                if (degreeGovernmentAnger == 3 || degreeGovernmentAnger == 4)
                    dayEnd.InformationTexts.Add(GetLabel("Руководство ищет нового кандидата на Ваше место."));
                if (degreeGovernmentAnger == 1 || degreeGovernmentAnger == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Руководство недовольно Вашей работой. Не забывайте об обязательных приказах."));

                if (productsDebts == 1)
                    dayEnd.InformationTexts.Add(GetLabel("Вы голодны."));
                else if (productsDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Вы умираете от голода."));

                if (heatingDebts == 1)
                    dayEnd.InformationTexts.Add(GetLabel("Вы озябли."));
                else if (heatingDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("Вы заболели."));

                if (rentDebts == 1 || rentDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("У Вас остались неоплаченные долги по счетам."));
                else if (rentDebts == 3 || rentDebts == 4)
                    dayEnd.InformationTexts.Add(GetLabel("Коммунальное хозяйство собирается выселить Вас из квартиры."));

                if (loyality > -10 && loyality <= -5)
                    dayEnd.InformationTexts.Add(GetLabel("Граждане протестуют. Они недовольны, что от них скрывают правду."));
            }
        }

        public void UpdateReprimandScore()
        {
            loyality += Level.Loyality;
            if (Level.ReprimandScore < 3)
            {
                if (flags[LevelNumber - 1].ContainsKey("MinistryIsSatisfied"))
                {
                    if (degreeGovernmentAnger > 0) --degreeGovernmentAnger;
                    flags[LevelNumber - 1]["MinistryIsSatisfied"] = true;
                    ++ministrySatisfactionsCount;
                }
            }
            else degreeGovernmentAnger += Level.ReprimandScore - 1;
        }

        public void ApplyExpenses()
        {
            foreach (var expence in dayEnd.Expenses)
            {
                if (expence.Marked) continue;
                switch (expence.Type)
                {
                    case ExpenseType.Rent: rentDebts += 3; break;
                    case ExpenseType.Products: productsDebts += 2; break;
                    case ExpenseType.Heating: heatingDebts += 2; break;
                    case ExpenseType.FirstStudies: flags[2]["TimasStudiesWerePaid"] = false; break;
                    case ExpenseType.Stranger: flags[3]["TheMainCharacterPaidForSilence"] = false; break;
                    case ExpenseType.Larisa: flags[3]["TheMainCharacterPaidLarisa"] = false; break;
                    case ExpenseType.Festival: flags[4]["MainCharacterWentToFestival"] = false; break;
                    case ExpenseType.Son: flags[5]["MainCharacterBoughtPresentForHisSon"] = false; break;
                    case ExpenseType.Galina: flags[6]["MainCharacterPaidGalina"] = false; break;
                    case ExpenseType.Medicine: flags[6]["MedicineWasDeliveredToWife"] = false; break;
                    case ExpenseType.SecondStudies: flags[8]["TimasStudiesWerePaid"] = false; break;
                    case ExpenseType.Operation: flags[9]["WifesOperationPaid"] = false; break;
                    case ExpenseType.Passports: flags[9]["MainCharacterBoughtFakePassports"] = false; break;
                }
            }
            if (rentDebts > 0) rentDebts--;
            if (productsDebts > 0) productsDebts--;
            if (heatingDebts > 0) heatingDebts--;
        }

        private string[] Read(string path)
        {
            var result = new List<string>();
            if (!File.Exists(path))
                return new string[0];
            var reader = new StreamReader(path);
            var line = reader.ReadLine();
            while (line != null)
            {
                result.Add(line);
                line = reader.ReadLine();
            }
            return result.ToArray();
        }

        private void UpdateElements()
        {
            if (LevelNumber >= 2)
                DecreesAreVisible = true;
            if (LevelNumber >= 5)
                TrendsAreVisible = true;
        }

        public string[] GetDecrees() => Read($"Decrees\\DecreesLevel{LevelNumber}.txt");

        public string[] GetTrends() => Read($"Trends\\TrendsLevel{LevelNumber}.txt");

        public object Clone() => MemberwiseClone();

        public GameOver CheckLoss(Control.ControlCollection controls)
        {
            if (degreeGovernmentAnger >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Fired, 450, 350),
                    "Вы уволены. Министерство цензуры и печати нашло Вам замену. " +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            if (loyality <= -10)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Fired, 450, 350),
                    "Вы не справились со своими обязанностями. Вас уволили. " +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            if (productsDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Dead, 450, 450),
                    "Вы потеряли сознание из-за сильного голода. Последнее, что Вы помните, — перед обмороком Вы мылись в душе...");

            if (rentDebts >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Expired, 500, 270),
                    "Вас выселили из квартиры. Вам пришлось вернуться в деревню к семье, " +
                    "но Вы не можете ежедневно ходить на работу из-за дальнего расстояния. " +
                    "Вас уволили. Вы обречены жить в бедности до конца своих дней...");

            if (heatingDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.DoctorRecommended, 450, 450),
                    "Вы тяжело заболели и не в состоянии работать. Вас уволили." +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            return null;
        }

        public void SetDifficulty(Difficulties difficulty)
        {
            this.difficulty = difficulty;
            if (difficulty is Difficulties.Normal)
            {
                loyalityFactor = 1;
                Money = 100;
                requiredLoyality = 45;
            }
            else
            {
                loyalityFactor = 2;
                Money = 200;
                requiredLoyality = 60;
            }
        }

        public void LoadFromJson()
        {
            if (!File.Exists(SavedData.name))
                throw new FileNotFoundException();

            var savedData = AuxiliaryMethods.GetSave();

            rent = savedData.Rent;
            productsCost = savedData.ProductsCost;
            heatingCost = savedData.HeatingCost;
            degreeGovernmentAnger = savedData.DegreeGovernmentAnger;
            rentDebts = savedData.RentDebts;
            productsDebts = savedData.ProductsDebts;
            heatingDebts = savedData.HeatingDebts;
            rent = savedData.Rent;
            productsCost = savedData.ProductsCost;
            heatingCost = savedData.HeatingCost;
            Date = new DateTime(savedData.Year, savedData.Mouth, savedData.Day);
            DecreesAreVisible = savedData.DecreesAreVisible;
            TrendsAreVisible = savedData.TrendsAreVisible;
            flags = savedData.GetFlags();
            money = savedData.Money;
            loyalityFactor = savedData.LoyalityFactor;
            loyality = savedData.Loyality;
            LevelNumber = savedData.LevelNumber;
            ministrySatisfactionsCount = savedData.MinistrySatisfactionsCount;
            requiredLoyality = savedData.RequiredLoyality;
        }

        private void SaveToJson()
        {
            var savedData = new SavedData
            {
                Flags = flags.Select(dict => new Flags(dict.Keys.ToArray(), dict.Values.ToArray())).ToArray(),
                Rent = rent,
                ProductsCost = productsCost,
                HeatingCost = heatingCost,
                DegreeGovernmentAnger = degreeGovernmentAnger,
                RentDebts = rentDebts,
                ProductsDebts = productsDebts,
                HeatingDebts = heatingDebts,
                Money = money,
                LoyalityFactor = loyalityFactor,
                Year = Date.Year,
                Mouth = Date.Month,
                Day = Date.Day,
                LevelNumber = LevelNumber,
                Loyality = loyality,
                Difficulty = difficulty,
                DecreesAreVisible = DecreesAreVisible,
                TrendsAreVisible = TrendsAreVisible,
                MinistrySatisfactionsCount = ministrySatisfactionsCount,
                RequiredLoyality = requiredLoyality
            };
            savedData.ToJson();
        }
    }
}
