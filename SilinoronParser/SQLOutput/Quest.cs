using SilinoronParser.Enums;

namespace SilinoronParser.SQLOutput
{
    public class Quest
    {
        public Quest()
        {
            RewardItem = new FourInts();
            RewardItemCount = new FourInts();
            RewardItemChoice = new SixInts();
            RewardItemChoiceCount = new SixInts();
            RewardRepFactionID = new FiveInts();
            RewardRepValueID = new FiveInts();
            RewardRepValue = new FiveInts();
            RequiredCreatureOrGOID = new FourInts();
            RequiredCreatureOrGOCount = new FourInts();
            ItemDropIntermediateID = new FourInts();
            ItemDropIntermediateCount = new FourInts();
            RequiredItemID = new SixInts();
            RequiredItemCount = new SixInts();
            ObjectiveTexts = new FourStrings();
            RewardCurrencyID = new FourInts();
            RewardCurrencyValue = new FourInts();
            RequiredCurrencyID = new FourInts();
            RequiredCurrencyValue = new FourInts();
        }

        public int Entry { get; set; }
        public QuestMethod Method { get; set; }
        public int Level { get; set; }
        public int MinLevel { get; set; }
        public int ZoneOrSort { get; set; }
        public QuestType Type { get; set; }
        public int SuggestedPlayers { get; set; }
        public int RepObjectiveFaction1 { get; set; }
        public int RepObjectiveValue1 { get; set; }
        public int RepObjectiveFaction2 { get; set; }
        public int RepObjectiveValue2 { get; set; }
        public int NextQuestID { get; set; }
        public int XPID { get; set; }
        public int RewardMoney { get; set; }
        public int RewardMoneyAtMaxLevel { get; set; }
        public int Spell { get; set; }
        public int SpellCast { get; set; }
        public int Honor { get; set; }
        public float HonorMultiplier { get; set; }
        public int SourceItemID { get; set; }
        public QuestFlag Flags { get; set; }
        public int Unk { get; set; }
        public int RewardTitleID { get; set; }
        public int PlayersSlain { get; set; }
        public int RewardTalentPoints { get; set; }
        public int RewardArenaPoints { get; set; }
        public int RewardSkillLineID { get; set; }
        public int RewardSkillPoints { get; set; }
        public int RewardFactionMask { get; set; }
        public int QuestGiverPortraitID { get; set; }
        public int QuestTurnInPortraitID { get; set; }
        public FourInts RewardItem { get; private set; }
        public FourInts RewardItemCount { get; private set; }
        public SixInts RewardItemChoice { get; private set; }
        public SixInts RewardItemChoiceCount { get; private set; }
        public FiveInts RewardRepFactionID { get; private set; }
        public FiveInts RewardRepValueID { get; private set; }
        public FiveInts RewardRepValue { get; private set; }
        public int PointMapID { get; set; }
        public float PointX { get; set; }
        public float PointY { get; set; }
        public int PointOption { get; set; }
        public string Title { get; set; }
        public string ObjectiveText { get; set; }
        public string Description { get; set; }
        public string EndText { get; set; }
        public string CompletionText { get; set; }
        public FourInts RequiredCreatureOrGOID { get; private set; }
        public FourInts RequiredCreatureOrGOCount { get; private set; }
        public FourInts ItemDropIntermediateID { get; private set; }
        public FourInts ItemDropIntermediateCount { get; private set; }
        public SixInts RequiredItemID { get; private set; }
        public SixInts RequiredItemCount { get; private set; }
        public int CriteriaSpellID { get; set; }
        public FourStrings ObjectiveTexts { get; private set; }
        public FourInts RewardCurrencyID { get; private set; }
        public FourInts RewardCurrencyValue { get; private set; }
        public FourInts RequiredCurrencyID { get; private set; }
        public FourInts RequiredCurrencyValue { get; private set; }
        public string QuestGiverPortraitText { get; set; }
        public string QuestGiverPortraitUnk { get; set; }
        public string QuestTurnInPortraitText { get; set; }
        public string QuestTurnInPortraitUnk { get; set; }
        public int SoundField1 { get; set; }
        public int SoundField2 { get; set; }

        public string ToSQL()
        {
            string sql = "REPLACE INTO questcache VALUES (";
            sql += Entry + ", ";
            sql += (int)Method + ", ";
            sql += Level + ", ";
            sql += MinLevel + ", ";
            sql += ZoneOrSort + ", ";
            sql += (int)Type + ", ";
            sql += SuggestedPlayers + ", ";
            sql += RepObjectiveFaction1 + ", ";
            sql += RepObjectiveValue1 + ", ";
            sql += RepObjectiveFaction2 + ", ";
            sql += RepObjectiveValue2 + ", ";
            sql += NextQuestID + ", ";
            sql += XPID + ", ";
            sql += RewardMoney + ", ";
            sql += RewardMoneyAtMaxLevel + ", ";
            sql += Spell + ", ";
            sql += SpellCast + ", ";
            sql += Honor + ", ";
            sql += HonorMultiplier + ", ";
            sql += SourceItemID + ", ";
            sql += (int)Flags + ", ";
            sql += Unk + ", ";
            sql += RewardTitleID + ", ";
            sql += PlayersSlain + ", ";
            sql += RewardTalentPoints + ", ";
            sql += RewardArenaPoints + ", ";
            sql += RewardSkillLineID + ", ";
            sql += RewardSkillPoints + ", ";
            sql += RewardFactionMask + ", ";
            sql += QuestGiverPortraitID + ", ";
            sql += QuestTurnInPortraitID + ", ";
            sql += RewardItem.ToSQL() + ", ";
            sql += RewardItemCount.ToSQL() + ", ";
            sql += RewardItemChoice.ToSQL() + ", ";
            sql += RewardItemChoiceCount.ToSQL() + ", ";
            sql += RewardRepFactionID.ToSQL() + ", ";
            sql += RewardRepValueID.ToSQL() + ", ";
            sql += RewardRepValue.ToSQL() + ", ";
            sql += PointMapID + ", ";
            sql += PointX + ", ";
            sql += PointY + ", ";
            sql += PointOption + ", ";
            sql += "\"" + Title + "\", ";
            sql += "\"" + ObjectiveText + "\", ";
            sql += "\"" + Description + "\", ";
            sql += "\"" + EndText + "\", ";
            sql += "\"" + CompletionText + "\", ";
            sql += RequiredCreatureOrGOID.ToSQL() + ", ";
            sql += RequiredCreatureOrGOCount.ToSQL() + ", ";
            sql += ItemDropIntermediateID.ToSQL() + ", ";
            sql += ItemDropIntermediateCount.ToSQL() + ", ";
            sql += RequiredItemID.ToSQL() + ", ";
            sql += RequiredItemCount.ToSQL() + ", ";
            sql += CriteriaSpellID + ", ";
            sql += ObjectiveTexts.ToSQL() + ", ";
            sql += RewardCurrencyID.ToSQL() + ", ";
            sql += RewardCurrencyValue.ToSQL() + ", ";
            sql += RequiredCurrencyID.ToSQL() + ", ";
            sql += RequiredCurrencyValue.ToSQL() + ", ";
            sql += "\"" + QuestGiverPortraitText + "\", ";
            sql += "\"" + QuestGiverPortraitUnk + "\", ";
            sql += "\"" + QuestTurnInPortraitText + "\", ";
            sql += "\"" + QuestTurnInPortraitUnk + "\", ";
            sql += SoundField1 + ", ";
            sql += SoundField2 + ");";
            return sql;
        }
    }
}
