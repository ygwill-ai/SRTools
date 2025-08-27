// Copyright (c) 2021-2025, JamXi JSG-LLC.
// All rights reserved.

// This file is part of SRTools.

// SRTools is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// SRTools is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with SRTools.  If not, see <http://www.gnu.org/licenses/>.

// For more information, please refer to <https://www.gnu.org/licenses/gpl-3.0.html>

using System;
using System.Text.RegularExpressions;
using SRTools.Views;

namespace SRTools.Depend
{
    public static class UrlHelper
    {
        public static void HandleUriActivation(Uri uri)
        {
            Console.Title = "𝑺𝑹𝑻𝒐𝒐𝒍𝒔 𝑼𝑹𝑰";
            Console.Clear();
            Logging.Write("检测到使用了URI参数");
            string uriString = uri.ToString().ToLower();

            bool isMatched = false;

            Check(@"^srtools:///startgame$", () =>
            {
                StartGame();
                isMatched = true;
            }, uriString);

            Check(@"^srtools:///startgame/([a-z0-9]+)/([a-z0-9]+)/([a-z0-9]+)$", () =>
            {
                var match = Regex.Match(uriString, @"^srtools:///startgame/([a-z0-9]+)/([a-z0-9]+)/([a-z0-9]+)$", RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    string region = match.Groups[1].Value;
                    string uid = match.Groups[2].Value;
                    string name = match.Groups[3].Value;
                    StartGameWithRegionUidName(region, uid, name);
                    isMatched = true;
                }
            }, uriString);

            if (!isMatched)
            {
                Logging.Write("未检测到任何支持的URI参数");
                Console.WriteLine("3秒后将退出程序...");
                System.Threading.Thread.Sleep(3000);
                Environment.Exit(1);
            }
        }

        private static void Check(string pattern, Action action, string input)
        {
            if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
            {
                action.Invoke();
            }
        }

        private static void StartGame()
        {
            GameStartUtil gameStartUtil = new GameStartUtil();
            gameStartUtil.StartGame();
            Environment.Exit(0);
        }

        private static async void StartGameWithRegionUidName(string region, string uid, string name)
        {
            string command = $"/RestoreUser {region} {uid} {name}";
            var result = await ProcessRun.SRToolsHelperAsync(command);
            Console.ReadLine();
        }
    }
}
