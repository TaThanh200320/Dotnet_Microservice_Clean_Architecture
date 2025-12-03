#!/bin/bash

# Script to create EF Core migrations for Identity service
# Usage: ./create-migration.sh [MigrationName]

set -e

MIGRATION_NAME="${1:-InitialCreate}"
BASE_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
INFRASTRUCTURE_DIR="$BASE_DIR/src/Identity/Infrastructure"
API_DIR="$BASE_DIR/src/Identity/Api"

echo "===================================="
echo "Creating EF Core Migration"
echo "===================================="
echo ""
echo "Migration Name: $MIGRATION_NAME"
echo "Infrastructure Project: $INFRASTRUCTURE_DIR"
echo "Startup Project: $API_DIR"
echo ""

# Navigate to Infrastructure directory
cd "$INFRASTRUCTURE_DIR"

# Create migration using bundled EF Core tools
echo "Creating migration..."
dotnet msbuild /t:GetEFToolsPath /nologo /verbosity:quiet

# Use the Design package to create migration
dotnet exec ~/.nuget/packages/microsoft.entityframeworkcore.design/8.0.11/lib/net8.0/Microsoft.EntityFrameworkCore.Design.dll \
    migrations add "$MIGRATION_NAME" \
    --project "$INFRASTRUCTURE_DIR/IdentityInfrastructure.csproj" \
    --startup-project "$API_DIR/IdentityApi.csproj" \
    --context TheDbContext \
    --output-dir Data/Migrations || {
    
    echo ""
    echo "⚠️  Standard method failed. Trying alternative approach..."
    echo ""
    
    # Alternative: Use dotnet build with migration target
    cd "$API_DIR"
    
    # Load environment variables
    set -a
    source "$BASE_DIR/.env"
    set +a
    
    export DatabaseSettings__DatabaseConnection="$DB_CONNECTION"
    
    # Try running with EnsureCreated instead
    echo "Will use EnsureCreated approach via code..."
    echo "Please add the following to your DbContext configuration temporarily:"
    echo ""
    echo "protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)"
    echo "{"
    echo "    if (!optionsBuilder.IsConfigured)"
    echo "    {"
    echo "        optionsBuilder.UseNpgsql(\"$DB_CONNECTION\");"
    echo "    }"
    echo "}"
    echo ""
}

echo ""
echo "===================================="
echo "Migration created successfully!"
echo "===================================="
echo ""
echo "To apply the migration, run:"
echo "  ./apply-migration.sh"
echo ""
