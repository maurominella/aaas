import matplotlib.pyplot as plt

# Data
companies = ['Company A', 'Company B', 'Company C', 'Company D']
profits = [1.2, 2.5, 3.0, 1.8]

# Create bar chart
plt.figure(figsize=(8, 6))
plt.bar(companies, profits, color=['blue', 'green', 'red', 'purple'])

# Add title and labels
plt.title('Operating Profit of Companies')
plt.xlabel('Companies')
plt.ylabel('Profit in Million Dollars')

# Save the plot to a file
plt.savefig('operating_profit_chart.png')

# Show the plot
plt.show()
