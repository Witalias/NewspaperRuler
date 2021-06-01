﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace NewspaperRuler
{
    public class Stats : ICloneable
    {
        public static string MonetaryCurrencyName { get; } = "ТОКЕНОВ";
        private readonly GraphicObject noteBackground;
        private readonly DayEnd dayEnd;

        private int degreeGovernmentAnger = 0;
        private int rentDebts = 0;
        private int productsDebts = 0;

        private int rent = 120;
        private int productsCost = 40;

        private int money;
        private int loyalityFactor = 1;

        public DateTime Date { get; private set; } = new DateTime(1987, 9, 26);

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

        public int Loyality { get; private set; }
        
        public LevelData Level { get; set; }

        public List<Dictionary<string, bool>> EventFlags { get; }

        public Stats(DayEnd dayEnd)
        {
            Money = 100;
            this.dayEnd = dayEnd;
            noteBackground = new GraphicObject(Properties.Resources.NoteBackground1, 750, 1000, 125);
            EventFlags = new List<Dictionary<string, bool>>
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
                },
                new Dictionary<string, bool>
                {
                    ["TheMainCharacterPaidForSilence"] = true,
                    ["TheMainCharacterPaidLarisa"] = true,
                    ["MainCharacterGaveOutAboutSecretEditorialOffice"] = false,
                }
            };
        }

        public void IncreaseLevelNumber() => LevelNumber++;

        public void SetFlagToTrue(string flag)
        {
            if (!EventFlags[LevelNumber - 1].ContainsKey(flag))
                throw new Exception($"The \"{flag}\" flag doesn't exist in the collection {EventFlags[LevelNumber - 1]}");
            EventFlags[LevelNumber - 1][flag] = true;
        }

        public void GoToNextLevel()
        {
            Date = Date.AddDays(1);
            Level = new LevelData(CreateNotes(), ArticleConstructor.ArticlesByLevel[LevelNumber - 1], loyalityFactor);
            CreateIntroduction();
            Level.BuildEventQueue();
        }

        private List<Note> CreateNotes()
        {
            var notes = new List<Note>();
            switch (LevelNumber)
            {
                case 1:
                    {
                        var text = "\tЗдравствуй, дорогой!" +
                            "\n\n\tЯ и наш сын Тимоша понимаем, что вступить на должность главного редактора гос. газеты – очень большая ответственность перед всей страной. " +
                            "\n\n\tМы планировали сходить в магазин за продуктами, но внеплановые обязательные налоги не позволили нам это сделать. Пожалуй, схожу на базар. Может, удастся договорится с давними знакомыми торговками." +
                            "\n\n\tУ Тимы на носу школьные выпускные экзамены, но я уверена, что он со всем справится. Он же у нас такой умничка!" +
                            "\n\n\tМы по тебе очень скучаем и сильно ждём!" +
                            "\n\n\tЦелую, твоя любимая жена";
                        notes.Add(new Note(noteBackground, text, "ОК"));
                        text = "\tТссс... Я раньше был на твоём месте. Меня уволили из-за малешей ошибочки. Руководство не должно узнать об этой записке. Разве не видишь? Аппарат управления прогнил изнутри." +
                            "\n\n\tГосударство устанавливает неведомое количество правил, в результате которых большинство правдивых статей не проходит к публикации." +
                            "\n\n\tНо люди хотят ВСЮ правду. Иначе говоря, отклоняя статьи, доверие читателей к государству теряется. " +
                            "Но доверие читателей - то что ХОЧЕТ видеть государство. От этого зависит и твоя зарплата. То есть, государство сознательно не даёт тебе спокойно работать." +
                            "\n\n\tЯ ничего такого не имею в виду. Просто хочу, чтобы ты пересмотрел то, до чего ты докатился сейчас. Хочу, чтоб ты принимал разумные решения. Работа государственным служащим - дело грязное." +
                            "\n\n\tИ помни, ты меня не знаешь. Тссс...";
                        notes.Add(new Note(noteBackground, text, "ОК"));
                        break;
                    }
                case 2:
                    {
                        var text = "\tПривет, красавчик!" +
                            "\n\n\tВстретимся в баре \"Алый цветок\" сегодня в 20:00";
                        notes.Add(new Note(noteBackground, text, "Пойти", "Игнорировать", "Вы пойдёте на свидание с таинственной незнакомкой", 
                            "Вы не пойдёте на свидание с таинственной незнакомкой", "MainCharacterWasOnDate"));
                        if (EventFlags[0]["ArticleAboutChampionWasApproved"])
                            text = "\tМногоуважаемый государственный служащий," +
                                "\n\n\tНе знаю, как статья о моих намерениях завести семью попала к Вам, но из-за Вас сегодня мне отказали в поездке за рубеж, где известные спортсмены со всего мира проводят мастер-классы по олимпийским видам спорта. " +
                                "Туда не берут беременных. Никто бы не узнал, ведь срок моей беременности только начался. Я не разглашала информацию об этом." +
                                "\n\n\tПрошу Вас никоим образом не вмешиваться в мою личную жизнь. Я пожалуюсь в Министерство социальной защиты, так и знайте." +
                                "\n\n\tГалина Руш";
                        else text = "\tМногоуважаемый государственный служащий," +
                                "\n\n\tНе знаю, как статья о моих намерениях завести семью попала в Ваши руки, но я безмерно благодарна Вам, что Вы не опубликовали статью и оставили это в секрете." +
                                "\n\n\tТеперь я смогу отправиться в поездку за рубеж, где известные спортсмены со всего мира проводят мастер-классы по олимпийским видам спорта. " +
                                "Туда не берут беременных, но об этом никто не узнает, ведь срок моей беременности только начался." +
                                "\n\n\tСпасибо Вам ещё раз! Примите от меня нескромный подарок в размере 50 ТОКЕНОВ." +
                                "\n\n\tГалина Руш";
                        notes.Add(new Note(noteBackground, text, "OK"));
                        break;
                    }
                case 3:
                    {
                        var text = new StringBuilder("\tСегодня мы, обычные люди, запускаем собственную газету, которая будет повествовать о РЕАЛЬНЫХ событиях в стране, а не о" +
                            " бессовестной лжи, которую забивают в наши головы чиновники.");
                        if (!EventFlags[1]["ArticleOnMassStarvationWasApproved"])
                            text.Append("\n\n\tПочему нам не рассказывают о массовом голоде, охватившем всю страну?");
                        if (!EventFlags[1]["ArticleAboutSalaryDelayWasApproved"])
                            text.Append("\n\n\tПочему от нас скрыли информацию о задержке зарплаты в Плиувиле?");
                        if (!EventFlags[1]["ArticleOnMassStarvationWasApproved"] || !EventFlags[1]["ArticleAboutSalaryDelayWasApproved"])
                            text.Append(" Да-да, мы об этом знаем и без гос. газеты.");
                        text.Append("\n\n\tПосмотрите на здание мэрии столицы — на нём же отчётливо кто-то написал \"Лжецы!\"." +
                            "\n\n\tНаша маленькая редакция будет работать в скрытном режиме. Просим не разглашать сведения о нашем существовании правительственным органам.");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK", 0));

                        text.Clear();
                        text.Append("\tИ снова привет, красавчик!");
                        if (EventFlags[1]["MainCharacterWasOnDate"])
                            text.Append("\n\n\tПомнишь нашу с тобой встречу в баре? У тебя был такой удивлённый взгляд... " +
                                "Жаль, что встреча так быстро завершилась, ведь ты почему-то поспешил уйти. " +
                                "Предлагаю встретиться повторно, но уже с обсуждением ближайшего будущего твоей семьи. ");
                        else text.Append("\n\n\tНетрудно было догадаться, что мне не стоило рассчитывать на нашу встречу в баре. " +
                            "Что ж, у меня есть предложение, которое тебя точно заинтересует. Оно касается ближайшего будущего твоей семьи. " +
                            "Предлагаю обсудить это наедине.");
                        text.Append("\n\n\tЯ вольна переселить всех вас в роскошные апартаменты, обеспечить достойной работой и, о да, я много чего ещё могу. " +
                            "Но взамен я бы хотела получить кое-что. Дай знать, когда будешь готов прийти." +
                            "\n\n\tЧао!");
                        notes.Add(new Note(noteBackground, text.ToString(), "ПОЙТИ", "ИГНОРИРОВАТЬ", "Вы пойдёте на встречу", "Вы не пойдёте на встречу", "MainCharacterWasOnDate"));

                        text.Clear();
                        text.Append("\tЗдравствуй, дорогой!");
                        if (EventFlags[1]["ArticleOnDryRationsWasApproved"])
                            text.Append("\n\n\tЯ видела опубликованную тобой новость о том, что всем выдадут индивидуальный рацион питания. И это случилось! " +
                                "Сегодня на почте мне действительно выдали 2 сухих пайка.");
                        else text.Append("\n\n\tСоседка сообщила, что на почте каждого ждёт индивидуальный рацион питания от государства. " +
                            "Я не видела эту новость в газете, и, если бы не соседка, я бы так и не узнала. На почте мне действительно выдали 2 сухих пайка.");
                        text.Append("\n\n\tНо, к великому сожалению, этого хватит лишь на пару дней. Я по-прежнему не могу найти работу. Я неоднократно посылала " +
                            "письма с жалобами в администрацию города. Мне ответили, что создание рабочих мест в процессе. Но пока их создают, не знаю, как долго мы протянем..." +
                            "\n\n\tЗавтра у Тимоши первый экзамен. Будем держать за него ручки. Это очень важно, ведь " +
                            "отличные результаты позволят ему поступить в высшую академию." +
                            "\n\n\tЦелую, твоя любимая жена.");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK"));

                        text.Clear();
                        text.Append("\tДобрый день, уважаемый." +
                            "\n\n\tМеня зовут Лариса. Я, конечно, не представитель МАМБА, но у меня есть связи с членами её команды. " +
                            "Я всячески оказываю содействие в их благих делах." +
                            "\n\n\tКак Вы знаете, МАМБА запретила нашей стране применять новейшее оружие массового поражения. Однако государство ослушалось. " +
                            "Они собираются применить оружие на войсках стран Андиплантийской коалиции, но держат это в секрете." +
                            "\n\n\tПожалуйста, опубликуйте следующую статью. Народ должен знать правду, а в интересах МАМБЫ предотвратить катастрофу, " +
                            "которая может случиться вследствие использования опасного оружия." +
                            "\n\n\tСпасибо");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK", 1));
                        break;
                    }
                case 4:
                    {
                        var text = new StringBuilder();
                        text.Append("\tНу что, красавчик, наконец-то ты понял, что я та ещё стерва. Ты, наверное, не понимаешь, но я тебе скажу. ");
                        if (EventFlags[2]["MainCharacterWasOnDate"])
                            text.Append("Ты пришёл на встречу, но вместо обсуждения делового предложения я вцепилась тебе в губы, после чего ты тут же поспешил уйти.");
                        else
                            text.Append("Ты явно не ожидал увидеть меня у своего подъезда. Я тут же вцепилась тебе в губы, но ты в недоумении поспешил скрыться за дверью.");
                        text.Append("\n\n\tТы же понимаешь, что никакого делового предложения нет? Я тебя обманула. " +
                            "Думаешь, это всё ? Хи-хи, не, это только начало. Ты от меня так просто не отделаешься." +
                            $"\n\n\tМой тайный агент запечатлел на память наш с тобой поцелуй. Если не возражаешь, я отправлю его твоей жене. Но ты можешь это предотвратить, если дашь мне 250 {MonetaryCurrencyName}. " +
                            "Я хочу денег. Много денег. И только ты можешь мне помочь, пупсик." +
                            "\n\n\tДаю тебе 2 дня. Обещаю, после этого я больше не потревожу ни тебя, ни твою семью." +
                            "\n\n\tИ не вздумай обращаться в полицию. " +
                            "Мой агент знает твоё местоположение и готов сыграть роль беспощадного убийцы, если потребуется." +
                            "\n\n\tЧао!");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK", 0));

                        text.Clear();
                        if (EventFlags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                            text.Append("\tЗдравствуйте, уважаемый." +
                                "\n\n\tЭто снова я, Лариса. Команда МАМБА выражает Вам огромную благодарность за публикацию требуемой статьи. " +
                                "Похоже, у военного руководства будут серьёзные проблемы…" +
                                "\n\n\tНу а теперь перейдём от официальной речи к разговорной. Я приехала в столицу по работе на пару дней и скоро уеду обратно в деревню. Я знаю о Вашей проблеме с шантажисткой. " +
                                "Вчера я была свидетелем вашей встречи и всё видела: и этот внезапный поцелуй, и странного типа, который вас фотографировал." +
                                "\n\n\tВаша жена мне много о Вас рассказывала, поэтому я легко Вас опознала. Всё верно, я её соседка." +
                                $"\n\n\tЯ всё улажу. Только дайте мне время и 25 {MonetaryCurrencyName} на кое-какие «карманные расходы». Не переживайте, они пойдут на благое дело. " +
                                $"Не ведитесь на провокации шантажистки, она должна получить по заслугам. Просто одолжите мне 25 {MonetaryCurrencyName}.");
                        else
                            text.Append("\tЗдравствуйте, уважаемый." +
                                "\n\n\tЭто снова я, Лариса. Очень жаль, что Вы отказались помогать команде МАМБА. Мы всё понимаем, государственный долг важнее." +
                                "\n\n\tМне много рассказывала о Вас жена. Всё верно, я её соседка. Я приехала в столицу по работе на пару дней и скоро отправлюсь обратно в деревню." +
                                "\n\n\tЯ легко Вас опознала вчера вечером. У Вас было свидание с красивой незнакомкой. " +
                                "Я не стану вмешиваться в Вашу личную жизнь, но знайте, что Вы поступили очень подло по отношению к своей жене. " +
                                "Не забывайте о своей семье: она самое дорогое, что у Вас есть." +
                                "\n\n\tПрощайте");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK"));

                        text.Clear();
                        text.Append("\tЗдравствуйте,");
                        if (EventFlags[0]["ArticleAboutChampionWasApproved"])
                            text.Append("\n\n\tЯ сожалею, что поведала о Вас в Министерство социальной защиты, ведь могла просто промолчать. " +
                                "Но сейчас мне нужна Ваша помощь!");
                        else
                            text.Append("\n\n\tВы мне очень помогли, когда отклонили новость о моей беременности. Но мне снова нужна Ваша помощь!");
                        text.Append("\n\n\tДело серьёзное. Пропал мой муж. " +
                            "Телефон заблокирован, от него и след простыл! Я не смогу построить семью без любимого." +
                            "\n\n\tЯ понимаю, что Вам не положено, но, пожалуйста, опубликуйте следующее объявление о пропаже. " +
                            "Поймите, это очень важно для меня. Обещаю, в долгу не останусь." +
                            "\n\n\tГалина Руш");
                        notes.Add(new Note(noteBackground, text.ToString(), "OK", 1));

                        text.Clear();
                        text.Append("\tДобрый день," +
                            "\n\n\tДо нас дошла информация о некой тайной редакции, которую организовали граждане. " +
                            "Они утверждают, что распространяют новости об истинных событиях, происходящих в стране. " +
                            "Государственная газета публикует только достоверные и самые накипевшие новости. " +
                            "Частные тайные организации, которые мы не можем никак контролировать, запрещены." +
                            "\n\n\tВы не замечали никаких подобного рода тайных организаций? Может, вчера Вам приходила от них записка?" +
                            "\n\n\tВ наших интересах устранить скрытные редакции, которые вольны забивать голову гражданам всем, чем захотят." +
                            "\n\n\tЕсли Вам есть, что сказать, я жду Вас у себя в кабинете после окончания смены." +
                            "\n\n\tС уважением," +
                            "\n\tМинистр цензуры и печати Хорош Оливер Леопольдович");
                        notes.Add(new Note(noteBackground, text.ToString(), "РАССКАЗАТЬ", "СКРЫТЬ",
                            "Вы расскажете министру всё о тайной редакции", "Вы оставите тайну о скрытой редакции в секрете", "MainCharacterGaveOutAboutSecretEditorialOffice"));
                        break;
                    }
            }
            return notes;
        }

        private void CreateIntroduction()
        {
            var text = new StringBuilder();
            var order = 0;
            switch (LevelNumber)
            {
                case 1:
                    text.Append("\tЗдравствуйте, редактор!" +
                        "\n\n\tВ ходе глобальной программы Перестройки мы освободили 1 рабочее место главного редактора в государственной газете, которое теперь занимаете Вы. " +
                        "Просто следуйте инструкциям, и до следующего перераспределения кадров рабочее место останется за Вами." +
                        "\n\n\tВо время исполнения должностных обязанностей Вы освобождаетесь от срочного призыва на военные действия. " +
                        "Для Вашего удобства мы также выделили Вам квартиру в центре столицы по адресу: ул. Торжественная, д. 47, кв. 14." +
                        "\n\n\tПосле прошедшей войны мы наблюдаем агрессивные настроения граждан: по ряду городов прошли восстания. В наших интересах показать людям, что ситуация налаживается, и Ваша сегодняшняя задача — публиковать только статьи ПОЗИТИВНОГО характера." +
                        "\n\n\tПеретаскивайте ШТАМПЫ на бумагу, чтобы сделать выбор." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    break;
                case 2:
                    text.Append("\tЗдравствуйте, редактор!");
                    if (EventFlags[0]["MinistryIsSatisfied"])
                        text.Append("\n\n\tМы выражаем восхищение Вашей способностью поднимать " +
                            "позитивные настроения граждан, однако проблем ещё предостаточно.");
                    else text.Append("\n\n\tМы недовольны Вашей работой: восстания продолжаются, " +
                        "а беженцев становится всё больше.");
                    text.Append("\n\n\tВступил в силу приказ №34.11, гласящий, что каждая статья обазана иметь ЗАГОЛОВОК. " +
                        "Отклоняйте все статьи без заголовков. Мы добавили данный приказ в ПЕРЕЧЕНЬ ДЕЙСТВУЮЩИХ ПРИКАЗОВ, " +
                        "который оставили на Вашем рабочем столе. Ознакомьтесь с ним и начинайте работу." +
                        "\n\n\tПродолжайте публиковать только статьи ПОЗИТИВНОГО характера." +
                        "\n\n\tОбращайте внимание на информацию о ПОГОДЕ: граждане любят знать точные прогнозы." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    break;
                case 3:
                    text.Append("\tЗдравствуйте, редактор!");
                    if (!EventFlags[1]["MinistryIsSatisfied"])
                        text.Append("\n\n\tМы недовольны Вашей работой. Ваша задача состояла в публикации исключительно статей " +
                            "позитивного характера, но Вы с ней не справились.");
                    text.Append("\n\n\tВероятно, Вы слышали о художестве проказников на стенах мэрии. Это недопустимо. " +
                        "Люди хотят слышать правду. Приказ №34.10 утрачивает свою силу: Вы можете публиковать статьи пессимистического характера. " +
                        "Однако мы не имеем права разглашать информацию о ходе военных конфликтов на границе, иначе у нас будут серьёзные проблемы. " +
                        "Пожалуйста, не забудьте проверить перечень приказов.");
                    if (!EventFlags[1]["ArticleOnProhibitionWeaponsWasApproved"])
                    {
                        text.Append("\n\n\tМы, несомненно, признательны Вам в том, что Вы не допустили к публикации новость о наставлениях МАМБА. " +
                            "Министерство безопасности планирует пусть новое оружие в ход. Просим продолжать держать это в секрете.");
                        if (EventFlags[1]["MinistryIsSatisfied"])
                        {
                            text.Append($"\n\tВы хорошо поработали. Ваша зарплата будет увеличена на 100 {MonetaryCurrencyName}.");
                            EventFlags[1]["SalaryIncreased"] = true;
                        }
                    }
                    else
                        text.Append("\n\n\tОпубликованная Вами новость о наставлениях МАМБА создало нам много проблем, ведь  " +
                            "Министерство военных дел планирует пусть новое оружие в ход. Пожалуйста, держите это в секрете.");
                        text.Append("\n\n\tНе забывайте проверять наличие ЗАГОЛОВКА в статьях." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    order = 1;
                    break;
                case 4:
                    text.Append("\tЗдравствуйте, редактор!");
                    if (EventFlags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                        text.Append("\n\n\tВы не сохранили в секрете информацию о применении оружия массового поражения. " +
                            "Мы в Вас разочарованы. Ваша зарплата сегодня будет урезана.");
                    text.Append("\n\n\tВ последнее время много разговоров идёт о «фальшивых новостях», которые публикуются в мусорных СМИ. " +
                        "Наша газета гарантирует полную достоверность выпускаемых новостей." +
                        "\n\n\tОдни из способов проверить фальшивость – сверить даты. Пожалуйста, сверяйте даты в статье (если таковые есть) с текущей сегодняшней датой. " +
                        "Отклоняйте статьи, в которых содержится устаревшая информация или информация «из будущего». Публикуйте только актуальные новости." +
                        "\n\n\tГраждане с чего-то взяли, что в гос. газете можно размещать объявления о пропажах, однако этим занимается иная организация. " +
                        "Пожалуйста, не забывайте также отклонять ОБЪЯВЛЕНИЯ О ПРОПАЖАХ ЛЮДЕЙ." +
                        "\n\n\tС уважением," +
                        "\n\tМинистерство цензуры и печати");
                    break;

            }
            if (text.ToString() == "") return;
            var title = Date.ToString("D");
            Level.Insert(order, new Article(ArticleConstructor.ArticleBackground, text.ToString(), title, order));
        }

        public void FinishLevel()
        {
            Loyality += Level.Loyality;
            Money += Level.Salary - Level.GetTotalFine();

            LevelNumber++;

            CreateWarnings();

            dayEnd.StatsTexts.Add(GetLabel($"Лояльность граждан:\t\t{Loyality}"));
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
                    if (!EventFlags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money += 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Подарок от Галины Руш:\t\t50"));
                    }
                    if (EventFlags[1]["MainCharacterWasOnDate"])
                    {
                        Money -= 30;
                        dayEnd.StatsTexts.Add(GetLabel($"Посещение бара \"Алый цветок\":\t\t-30"));
                    }
                    break;
                case 3:
                    if (EventFlags[0]["ArticleAboutChampionWasApproved"])
                    {
                        Money -= 50;
                        dayEnd.StatsTexts.Add(GetLabel($"Штраф от Министерства социальной защиты:\t\t-50"));
                        dayEnd.InformationTexts.Add(GetLabel("Министерство социальной защиты налагает штраф за нарушение"));
                        dayEnd.InformationTexts.Add(GetLabel("неприкосновенности частной жизни гражданки Галины Руш."));
                    }
                    if (EventFlags[1]["SalaryIncreased"])
                    {
                        Money += 100;
                        dayEnd.StatsTexts.Add(GetLabel($"Бонус к зарплате:\t\t100"));
                        dayEnd.InformationTexts.Add(GetLabel("Ваша сегодняшняя зарплата увеличена за хорошую работу."));
                    }
                    break;
                case 4:
                    productsCost += 10;
                    dayEnd.InformationTexts.Add(GetLabel("Цены на продукты повысились."));
                    dayEnd.Expenses.Add(new Expense($"Плата за молчание:\t\t250 {MonetaryCurrencyName}", 250, ExpenseType.Stranger));
                    if (EventFlags[2]["ArticleOnProhibitionWeaponsWasApproved"])
                    {
                        Money -= 75;
                        dayEnd.StatsTexts.Add(GetLabel($"Вычет из зарплаты:\t\t-75"));
                        dayEnd.Expenses.Add(new Expense($"План Ларисы:\t\t25 {MonetaryCurrencyName}", 25, ExpenseType.Larisa));
                    }
                    break;
            }
            dayEnd.StatsTexts.Add(GetLabel($"Итого:\t\t{Money} {MonetaryCurrencyName}"));

            dayEnd.Expenses.Add(new Expense($"Квартплата:\t\t{rent} {MonetaryCurrencyName}", rent, ExpenseType.Rent));
            dayEnd.Expenses.Add(new Expense($"Продукты:\t\t{productsCost} {MonetaryCurrencyName}", productsCost, ExpenseType.Products));

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

                if (rentDebts == 1 || rentDebts == 2)
                    dayEnd.InformationTexts.Add(GetLabel("У Вас остались неоплаченные долги по счетам."));
                else if (rentDebts == 3 || rentDebts == 4)
                    dayEnd.InformationTexts.Add(GetLabel("Коммунальное хозяйство собирается выселить Вас из квартиры."));
            }
        }

        public void UpdateReprimandScore()
        {
            if (Level.ReprimandScore < 3)
            {
                if (EventFlags[LevelNumber - 1].ContainsKey("MinistryIsSatisfied"))
                {
                    if (degreeGovernmentAnger > 0) degreeGovernmentAnger--;
                    EventFlags[LevelNumber - 1]["MinistryIsSatisfied"] = true;
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
                    case ExpenseType.Stranger: EventFlags[3]["TheMainCharacterPaidForSilence"] = false; break;
                    case ExpenseType.Larisa: EventFlags[3]["TheMainCharacterPaidLarisa"] = false; break;
                }
            }
            if (rentDebts > 0) rentDebts--;
            if (productsDebts > 0) productsDebts--;
        }

        public string[] GetDecrees()
        {
            var result = new List<string>();
            if (!File.Exists($"Decrees\\DecreesLevel{LevelNumber}.txt"))
                return new string[0];
            var reader = new StreamReader($"Decrees\\DecreesLevel{LevelNumber}.txt");
            var line = reader.ReadLine();
            while (line != null)
            {
                result.Add(line);
                line = reader.ReadLine();
            }
            return result.ToArray();
        }

        public object Clone() => MemberwiseClone();

        public GameOver CheckLoss(Control.ControlCollection controls)
        {
            if (degreeGovernmentAnger >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Fired, 450, 350),
                    "Вы уволены. Министерство цензуры и печати нашло Вам замену. " +
                    "Вы вернулись к своей семье, где Вам суждено жить в бедности до конца своих дней...");

            if (productsDebts >= 3)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Dead, 450, 450),
                    "Вы потеряли сознание из-за сильного голода. Последнее, что Вы помните, — перед обмороком Вы мылись в душе...");

            if (rentDebts >= 5)
                return new GameOver(controls, new GraphicObject(Properties.Resources.Expired, 500, 270),
                    "Вас выселили из квартиры. Вам пришлось вернуться в деревню к семье, " +
                    "но Вы не можете ежедневно ходить на работу из-за дальнего расстояния. " +
                    "Вас уволили. Вы обречены жить в бедности до конца своих дней...");

            return null;
        }

        public void SetDifficulty(Difficulties difficulty)
        {
            if (difficulty is Difficulties.Normal) loyalityFactor = 1;
            else loyalityFactor = 2;
        }
    }
}
