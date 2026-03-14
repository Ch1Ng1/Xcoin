# Cryptocurrency Price Tracker

A modern, responsive web application built with Blazor WebAssembly that displays cryptocurrency monthly price data with interactive charts and comprehensive statistics.

![GitHub Pages](https://img.shields.io/badge/GitHub%20Pages-Deployed-brightgreen)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-purple)
![Chart.js](https://img.shields.io/badge/Chart.js-4.4.0-orange)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-blue)

## 🌟 Live Demo

[View Live Application](https://ching1.github.io/Xcoin/)

## 📋 Features

- **📊 Interactive Charts**: Beautiful line charts showing monthly price trends using Chart.js
- **📈 Statistics Dashboard**: Key metrics including average, highest, and lowest prices
- **📋 Data Table**: Comprehensive monthly price data in an easy-to-read table format
- **📱 Responsive Design**: Works perfectly on desktop, tablet, and mobile devices
- **⚡ Fast Loading**: Built with Blazor WebAssembly for optimal performance
- **🎨 Modern UI**: Clean, professional interface using Bootstrap 5

## 🚀 Quick Start

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- A web browser

### Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/Ch1Ng1/Xcoin.git
   cd Xcoin/XCoinBlazor
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Open your browser** and navigate to `https://localhost:5000` (or the URL shown in the console)

## 🏗️ Architecture

### Project Structure

```
XCoinBlazor/
├── Models/
│   └── DataModels.cs          # Data models for cryptocurrency data
├── Services/
│   └── CryptoDataService.cs   # Service for loading and processing data
├── Pages/
│   └── Home.razor            # Main dashboard page
├── wwwroot/
│   ├── data/
│   │   ├── month-stats.json  # Monthly statistics data
│   │   └── resp.json         # Daily price data
│   ├── js/
│   │   └── chartInterop.js   # Chart.js integration
│   └── index.html            # Main HTML file
└── Program.cs                # Application entry point
```

### Data Processing

The application processes cryptocurrency data through several steps:

1. **Data Loading**: JSON files are loaded from `wwwroot/data/`
2. **Data Processing**: Daily prices are aggregated into monthly averages
3. **Statistics Calculation**: Min, max, and average values are computed
4. **Chart Rendering**: Data is passed to Chart.js for visualization

### Technologies Used

- **Blazor WebAssembly**: For building the client-side web application
- **Chart.js**: For creating interactive charts
- **Bootstrap 5**: For responsive UI components
- **.NET 8**: Runtime and development platform
- **C#**: Primary programming language

## 📊 Data Sources

The application uses historical cryptocurrency data stored in JSON format:

- `month-stats.json`: Contains monthly statistics (min, max, average prices)
- `resp.json`: Contains daily price data for price calculations

## 🚀 Deployment

### GitHub Pages (Recommended)

The application is automatically deployed to GitHub Pages using GitHub Actions:

1. **Push to main branch** - The workflow automatically triggers
2. **GitHub Actions builds** the application
3. **Deploy to GitHub Pages** - Available at `https://[username].github.io/[repository-name]`

### Manual Deployment

To deploy manually:

1. **Build for production**
   ```bash
   dotnet publish -c Release -o dist
   ```

2. **Serve the `dist/wwwroot` folder** using any static web server

### Other Hosting Options

- **Azure Static Web Apps**
- **AWS S3 + CloudFront**
- **Vercel**
- **Netlify**

## 🔧 Development

### Adding New Features

1. **Data Models**: Add new models in `Models/DataModels.cs`
2. **Services**: Extend `CryptoDataService.cs` for new functionality
3. **UI Components**: Create new Razor components in `Pages/` or `Shared/`
4. **Styling**: Modify `wwwroot/css/app.css` for custom styles

### Testing

Run the application locally and verify:
- Data loads correctly
- Charts render properly
- Responsive design works on different screen sizes
- All interactive features function as expected

## 📈 Performance

- **Bundle Size**: Optimized for fast loading
- **Lazy Loading**: Components load on demand
- **Caching**: Data is processed efficiently
- **Responsive**: Optimized for all device types

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is open source and available under the [MIT License](LICENSE).

## 🙏 Acknowledgments

- **Chart.js** for the amazing charting library
- **Bootstrap** for the responsive UI framework
- **.NET Blazor** for the WebAssembly platform
- **GitHub Pages** for free hosting

## 📞 Support

If you have any questions or issues:

1. Check the [Issues](https://github.com/Ch1Ng1/Xcoin/issues) page
2. Create a new issue with detailed information
3. Contact the maintainers

---

**Built with ❤️ using Blazor WebAssembly**