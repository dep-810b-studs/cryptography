﻿using System.Collections.Generic;
using BlazorDownloadFile;
using Cryptography.Algorithms;
using Cryptography.Algorithms.Rijandel;
using Cryptography.Algorithms.Symmetric;
using Cryptography.Algorithms.Symmetric.CipherManager;
using Cryptography.Algorithms.Symmetric.CipherStrategy;
using Cryptography.Algorithms.Symmetric.CipherSystem;
using Cryptography.Algorithms.Symmetric.Padding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.JSInterop;

namespace Cryptography.WebInterface.SymmetricCipher
{
    public static class ConfigurationsExtensions
    {
        public static void ConfigureRijndel(this IServiceCollection services)
        {
            services.AddSingleton<ISymmetricCipher, RijandelCipher>();
            services.AddSingleton<Dictionary<SymmetricCipherMode, ICipherStrategy>>(new Dictionary<SymmetricCipherMode, ICipherStrategy>()
            {
                [SymmetricCipherMode.ElectronicCodeBook] = new ElectronicCodeBookStrategy(),
                [SymmetricCipherMode.CipherBlockChaining] = new CipherBlockChainingStrategy(),
                [SymmetricCipherMode.CipherFeedback] = new CipherFeedbackStrategy(),
                [SymmetricCipherMode.OutputFeedback] = new OutputFeedbackStrategy()
            });
            services.AddSingleton<IPaddingService, PaddingService>();
            services.AddSingleton<ISymmetricCipherManager, SymmetricCipherManager>();
            services.AddSingleton<ISymmetricSystem, SymmetricSystem>();
            services.AddBlazorDownloadFile();
        }
    }
}