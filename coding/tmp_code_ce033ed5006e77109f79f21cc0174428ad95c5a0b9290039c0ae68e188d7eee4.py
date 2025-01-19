import matplotlib.pyplot as plt

# Data for the bar chart
companies = ['Company A', 'Company B', 'Company C', 'Company D']
profits = [1.2, 2.5, 3.0, 1.8]

# Create the bar chart
plt.figure(figsize=(8, 6))
plt.bar(companies, profits, color=['blue', 'green', 'red', 'purple'])

# Add title and labels
plt.title('Operating Profit of Companies')
plt.xlabel('Companies')
plt.ylabel('Profit in Million Dollars')

# Save the bar chart as an image file
plt.savefig('operating_profit_chart.png')

# Show the plot (optional, if you want to see it in a window)
plt.show()
