export const timeZone: TimeZone[] = [
  // UTC & GMT
  { name: "UTC", value: "UTC" },
  { name: "Greenwich Mean Time (GMT)", value: "GMT" },

  // North America
  { name: "Eastern Time (US & Canada)", value: "America/New_York" },
  { name: "Central Time (US & Canada)", value: "America/Chicago" },
  { name: "Mountain Time (US & Canada)", value: "America/Denver" },
  { name: "Pacific Time (US & Canada)", value: "America/Los_Angeles" },
  { name: "Alaska Time", value: "America/Anchorage" },
  { name: "Hawaii Time", value: "Pacific/Honolulu" },

  // South America
  { name: "Argentina Time", value: "America/Argentina/Buenos_Aires" },
  { name: "Brazil (São Paulo)", value: "America/Sao_Paulo" },

  // Europe
  { name: "London (UK)", value: "Europe/London" },
  { name: "Berlin (Germany)", value: "Europe/Berlin" },
  { name: "Paris (France)", value: "Europe/Paris" },
  { name: "Madrid (Spain)", value: "Europe/Madrid" },
  { name: "Moscow (Russia)", value: "Europe/Moscow" },

  // Middle East
  { name: "Dubai (UAE)", value: "Asia/Dubai" },
  { name: "Tehran (Iran)", value: "Asia/Tehran" },
  { name: "Istanbul (Turkey)", value: "Europe/Istanbul" },

  // Asia
  { name: "India Standard Time", value: "Asia/Kolkata" },
  { name: "Pakistan Standard Time", value: "Asia/Karachi" },
  { name: "Bangladesh Standard Time", value: "Asia/Dhaka" },
  { name: "Thailand Time", value: "Asia/Bangkok" },
  { name: "China Standard Time", value: "Asia/Shanghai" },
  { name: "Hong Kong Time", value: "Asia/Hong_Kong" },
  { name: "Japan Standard Time", value: "Asia/Tokyo" },
  { name: "South Korea Time", value: "Asia/Seoul" },

  // Australia & Pacific
  { name: "Australia Eastern Time (Sydney)", value: "Australia/Sydney" },
  { name: "New Zealand Time (Wellington)", value: "Pacific/Auckland" },

  // Africa
  { name: "South Africa Standard Time", value: "Africa/Johannesburg" },
  { name: "Nairobi (Kenya)", value: "Africa/Nairobi" }
];

export class TimeZone {
  name: string | undefined;
  value: string | undefined;
}
