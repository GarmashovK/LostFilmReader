using System;
using LostFilmLibrary.Serials;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace LostFilmLibraryTest
{
    [TestClass]
    public class SerialsTest
    {
        [TestMethod]
        public async Task Test_SuccessfulLoadingOfSerialsList()
        {
            LostFilmLibrary.LFOptions.Cookies = null;
            var serialsList = new SerialsList();
            await serialsList.LoadAsync();

            Assert.AreEqual(serialsList.Count != 0, true);
        }

        [TestMethod]
        public async Task Test_SuccessfulSerialPageLoading()
        {
            LostFilmLibrary.LFOptions.Cookies = null;

        }
    }
}