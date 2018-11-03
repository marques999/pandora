using System;
using System.Collections.Generic;
using System.Text;

using SmoothDownloader.Smooth;

namespace SmoothDownloader.Mkv
{
    /// <summary>
    /// </summary>
    public class TrackEntry
    {
        /// <summary>
        /// </summary>
        private const string LanguageCodes = "abkaceachadaadyadyaarafhafrafaainakaakkalbgswalealggswtutamhanpapaaraargarparwarmrupartrupasmastastathausmapavaaveawaaymazeastbanbatbalbambaibadbntbasbakbaqbtkbejbejbelbembenberbhobihbikbynbinbisbynzblzblzblnobbosbrabrebugbulbuaburcadspacatcaucebcelcaikhmchgcmcchachechrnyachychbnyachichnchpchozhachuchuchkchvnwcnwcsycrarcopcorcoscremuscrpcpecpfcppcrhcrhhrvcusczedakdandardelchpdivzzazzadindivdoidgrdraduadutdumdyudzofrsbinefiegyekaelxengenmangmyvepoesteweewofanfatfaofijfilfinfiudutfonfrefrmfrofurfulgaaglacarglgluggaygbagezgeogerndsgmhgohgemkikgilgongorgotgrbgrcgrekalgrngujgwihaihathathauhawhebherhilhimhinhmohithmnhunhupibaiceidoiboijoiloarcsmnincineindinhinaileikuipkiraglemgasgairoitajpnjavkacjrbjprkbdkabkackalxalkamkankaupamkaakrckrlkarkascsbkawkazkhakhikhokikkmbkinzzakirzzatlhkomkonkokkorkoskpekrokuakumkurkrukutkuakirladlahlamdaylaolatlavastltzlezlimlimlimlinlitjbondsndsdsblozlublualuismjlunluolusltzrupmacmadmagmaimakmlgmaymaldivmltmncmdrmanmnimnoglvmaoarnarnmarchmmahmwrmasmynmenmicmicminmwlmohmdfrumrummkhhmnlolmonmosmulmunnqonahnaunavnavndenblndonapnewnepnewnianicssaniuzxxnognonnaindefrrsmensonornobnnozxxnubiiinymnyanynnnonyonziileociproarcxalojichuchunwcchuoriormosaossossotopalpauplipampagpanpappaapusnsoperpeophiphnfilponpolporprapropanpusquerajraprarqaaroarumrohromrunruskhosalsamsmismosadsagsansatsrdsasndsscoglaselsemnsosrpsrrshnsnaiiiscnsidsgnblasndsinsinsitsiosmsdenslasloslvsogsomsonsnkwennsosotsainblaltsmaspasrnsuksuxsunsusswasswswegswsyrtgltahtaitgktmhtamtatteltertetthatibtigtirtemtivtlhtlitpitkltogtontsitsotsntumtupturotatuktvltyvtwiudmugauigukrumbmisundhsburduiguzbvaicatvenvievolvotwakwlnwarwaswelfryhimwalwalwolxhosahyaoyapyidyorypkzndzapzzazzazenzhazulzun";

        /// <summary>
        /// </summary>
        private readonly byte[] _codecPrivate;

        /// <summary>
        /// </summary>
        private readonly byte[] _infoBytes;

        /// <summary>
        /// </summary>
        public LanguageId Language = LanguageId.English;

        /// <summary>
        /// </summary>
        public MkvCodec MkvCodec;

        /// <summary>
        /// </summary>
        public string Name;

        /// <summary>
        /// </summary>
        public ulong TrackNumber;

        /// <summary>
        /// </summary>
        public MkvTrackType TrackType;

        /// <summary>
        /// </summary>
        /// <param name="trackType"></param>
        /// <param name="infoBytes"></param>
        /// <param name="mkvCodec"></param>
        /// <param name="codecPrivate"></param>
        public TrackEntry(MkvTrackType trackType, byte[] infoBytes, MkvCodec mkvCodec, byte[] codecPrivate)
        {
            MkvCodec = mkvCodec;
            TrackType = trackType;
            _infoBytes = infoBytes;
            _codecPrivate = codecPrivate;
        }

        /// <summary>
        /// </summary>
        /// <param name="languageId"></param>
        /// <returns></returns>
        private static string GetLanguageCode(LanguageId languageId)
        {
            var identifier = (int)languageId * 3;

            if (languageId < 0 || identifier >= LanguageCodes.Length)
            {
                throw new Exception();
            }

            return LanguageCodes.Substring(identifier, 3);
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            if (TrackNumber == 0)
            {
                throw new Exception();
            }

            var contents = new List<byte[]>
            {
                MkvUtils.GetEeBytes(MkvIdentifier.TrackNumber, MkvUtils.GetVintBytes(TrackNumber)),
                MkvUtils.GetEeBytes(MkvIdentifier.TrackUid, MkvUtils.GetVintBytes(TrackNumber)),
                MkvUtils.GetEeBytes(MkvIdentifier.TrackType, MkvUtils.GetVintBytes((ulong) TrackType)),
                MkvUtils.GetEeBytes(MkvIdentifier.FlagEnabled, MkvUtils.GetVIntForFlag(true)),
                MkvUtils.GetEeBytes(MkvIdentifier.FlagDefault, MkvUtils.GetVIntForFlag(true)),
                MkvUtils.GetEeBytes(MkvIdentifier.FlagForced, MkvUtils.GetVIntForFlag(false)),
                MkvUtils.GetEeBytes(MkvIdentifier.FlagLacing, MkvUtils.GetVIntForFlag(true))
            };

            if (string.IsNullOrEmpty(Name) == false)
            {
                contents.Add(MkvUtils.GetEeBytes(MkvIdentifier.Name, Encoding.UTF8.GetBytes(Name)));
            }

            if (Language != LanguageId.English)
            {
                contents.Add(MkvUtils.GetEeBytes(MkvIdentifier.Language, Encoding.ASCII.GetBytes(GetLanguageCode(Language))));
            }

            contents.Add(MkvUtils.GetEeBytes(MkvIdentifier.CodecId, Encoding.ASCII.GetBytes(MkvUtils.GetStringForCodecId(MkvCodec))));

            if (_codecPrivate != null)
            {
                contents.Add(MkvUtils.GetEeBytes(MkvIdentifier.CodecPrivate, _codecPrivate));
            }

            contents.Add(_infoBytes);

            return MkvUtils.GetEeBytes(MkvIdentifier.TrackEntry, Utils.CombineByteArrays(contents));
        }
    }
}