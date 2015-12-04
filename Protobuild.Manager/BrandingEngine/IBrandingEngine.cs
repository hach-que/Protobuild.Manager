﻿namespace Protobuild.Manager
{
    internal interface IBrandingEngine
    {
        /// <summary>
        /// The name of the product, as displayed to the user.
        /// </summary>
        string ProductName { get; }

        /// <summary>
        /// The name of the storage folder to use when storing information
        /// inside the Application Data (or Linux / Mac equivalent).
        /// </summary>
        string ProductStorageID { get; }

        /// <summary>
        /// The online RSS feed URL that should be used for displaying news.
        /// </summary>
        string RSSFeedURL { get; }

#if PLATFORM_WINDOWS
        System.Drawing.Icon WindowsIcon { get; }
#endif
    }
}