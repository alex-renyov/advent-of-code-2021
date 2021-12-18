package ru.renev.adventofcode2021

class Day3 {
    fun part1(): Int {
        println("Day 3 part 1")

        val lines = Day3Input.values.split("\n")
        val length = lines[0].length

        val gamma = CharArray(length)

        for (i in 0 until length) {
            val quantities = lines.map { it[i] }
                .groupBy { it }
                .map { it.key to it.value.count() }
                .associate { it }
            gamma[i] = if (quantities.getOrDefault('1', 0) >= quantities.getOrDefault('0', 0)) '1' else '0'
        }

        val epsilon = gamma.map { if (it == '0') '1' else '0' }.toCharArray()

        val gammaValue = String(gamma).toInt(2)
        val epsilonValue = String(epsilon).toInt(2)

        return gammaValue * epsilonValue
    }

    fun part2(): Int {
        println("Day 3 part 2")

        val lines = Day3Input.values.split("\n")
        val length = lines[0].length

        val oxygen = calculateNumber(lines, length) { count0, count1 -> if (count1 >= count0) '1' else '0' }
        val co2 = calculateNumber(lines, length) { count0, count1 -> if (count0 <= count1) '0' else '1' }

        return oxygen * co2
    }

    private fun calculateNumber(source: List<String>, length: Int, charComparison: (Int, Int) -> Char): Int {
        var currentLines = source

        for (i in 0 until length) {
            val counts = currentLines.map { it[i] }
                .groupBy { it }
                .map { it.key to it.value.count() }
                .associate { it }

            val foundChar = charComparison(counts.getOrDefault('0', 0), counts.getOrDefault('1', 0))
            currentLines = currentLines.filter { it[i] == foundChar }

            if (currentLines.count() == 1) {
                return currentLines.first().toInt(2)
            }
        }

        throw RuntimeException("Something went wrong")
    }
}
