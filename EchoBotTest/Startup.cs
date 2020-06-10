﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using LegalBotTest.Bots;
using Microsoft.Extensions.DependencyInjection.Extensions;
using LegalBotTest.Services;
using Microsoft.Bot.Builder.Azure;
using LegalBotTest.Dialogs;
//using LegalBotTest.Models.Interfaces;

namespace LegalBotTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Create the Bot Framework Adapter with error handling enabled.
            services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();

            //Configure State
            ConfigureState(services);

            //Configure Dialogs
            ConfigureDialogs(services);


            // Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
            //services.AddTransient<IBot, GreetingBot>();
            services.AddTransient<IBot, DialogBot<MainDialog>>();
            //services.AddSingleton<IFileUtility, DiskFileUtility>();
        }

        public void ConfigureState(IServiceCollection services)
        {
            //Create the storage we'll using for User and Conversation state. (Memory is great for testing purposes.)
            //services.AddSingleton<IStorage,MemoryStorage>();

            //wanjiku 150520
            //var storageAcount = "DefaultEndpointsProtocol=https;AccountName=psdemos1305;AccountKey=NNB9WgNgXPUk6xxHwnFQ93f0ZXi+B6juGYjnP3KNz624vKChSCj+KeCTWt01w8FBQRbvFi2ro3utjkNH2+Us6A==;EndpointSuffix=core.windows.net";
            //var storageContainer = "mystatedata";

            //services.AddSingleton<IStorage>(new AzureBlobStorage(storageAcount, storageContainer));

            //end wanjiku 150520
            services.AddSingleton<IStorage, MemoryStorage>();

            //Create user state 
            services.AddSingleton<UserState>();
            //Create conversation state
            services.AddSingleton<ConversationState>();
            //Create an instance of the state service
            services.AddSingleton<BotStateService>();
        }

        public void ConfigureDialogs(IServiceCollection services)
        {
            services.AddSingleton<MainDialog>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseWebSockets();
            //app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
